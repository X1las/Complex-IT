import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import '../App.css';
import {useAddTitleToHistory} from './history.jsx';
import DisplayTitleItem from '../services/titlefunctions.jsx';

export  const NL_API = 'https://www.newtlike.com:3000';
export  const TMDB_API = 'https://api.themoviedb.org';
export  const API_KEY = '6d931505602649b6ba683649d9af5d82';

 async function SearchNL(query) {

  const url = `${NL_API}/api/titles?search=${(query)}`;

    if (!query || query.trim() === '') return [];

    const response = await fetch(url);
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    
  const data = await response.json();
  
  const items = data.items || [];

  return items;
}

// TMDB fallback
async function searchTMDB(tconst) {
  const url = `${TMDB_API}/3/find/${tconst}?external_source=imdb_id&api_key=${API_KEY}`;

  const response = await fetch(url);
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
    
  const data = await response.json();
  const movie = data.movie_results?.[0];
  const tv = data.tv_results?.[0];
  
  return (movie?.poster_path || tv?.poster_path) || null;
}
// Fetch poster New
async function getPosterFromNewtlike(tconst) {
  try {
    const url = `${NL_API}/api/titles/${(tconst)}`;
    const response = await fetch(url);
    
    if (!response.ok) return {};
    
    const data = await response.json();

    const posterUrl = data.posterUrl;
    if (!posterUrl || posterUrl.trim() === '') return null;

    return data.posterUrl || null;
  } catch (err) {
    console.error(`Fejl ved hentning af poster for ${tconst}:`, err.message);
    return null;
  }
}

async function searchTitlePosters(query) {
  const items = await SearchNL(query);
  
  const resultsWithPosters = await Promise.all(items.map(async (item) => {
    let posterUrl = null;
    
    if (item.id) {
      posterUrl = await getPosterFromNewtlike(item.id);
      
      
      if (posterUrl) {
        console.log(`[NEWTLIKE] ${item.title} (${item.id})`);
      } 
      //Fallback til TMDB hvis ingen poster i database
      else {
        const posterPath = await searchTMDB(item.id);
        posterUrl = posterPath ? `https://image.tmdb.org/t/p/w342${posterPath}` : null;
        
        if (posterUrl) {

          console.log(`[TMDB] ${item.title} (${item.id})`);
        } else {

          console.log(`[INGEN] ${item.title} (${item.id})`);
        }
      }
    }
    
    return {
        ...item,
        poster_url: posterUrl
      };
  }));
  
  return resultsWithPosters;
}


 
const Search = () => {
  const { q } = useParams();
  const [items, setItems] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const { addToHistory } = useAddTitleToHistory();

  //const { items, isLoading, error } = searchTMDBOnly(q);

  useEffect(() => {
    if (!q || q.trim() === '') {
      return;
    }
    setIsLoading(true);
    searchTitlePosters(q)
      .then(responseItems => setItems(responseItems))
      .catch(err => setError(err))
      .finally(() => setIsLoading(false));
  }, [q]);


const HistoryHandler = async (titleId) => {
  return addToHistory(titleId)
}


  if (isLoading) 
    return <div style={{padding: 20}}>Loading...</div>;
  if (error) {
    console.error('Search fejl:', error);
    return <div className='pagestuff'>No results found for "{q || ''}"</div>;
  }

  console.log('Search results:', items); // Debug log
  console.log('DisplayTitleItem component:', DisplayTitleItem); // Debug log

  return (
    <div className='search-results-container' style={{ padding: '20px', backgroundColor: '#1a1a1a', minHeight: '100vh' }}>
      <h2 style={{ color: 'white', marginBottom: '20px' }}>Search Results for "{q}"</h2>
      {(items || []).length === 0 ? (
        <div style={{ color: 'white' }}>No results found</div>
      ) : (
        (items || []).map(movie => {
          console.log('Rendering movie:', movie.id); // Debug log
          return <DisplayTitleItem key={movie.id} tconst={movie.id} />;
        })
      )}
    </div>
  );
};

export default Search;