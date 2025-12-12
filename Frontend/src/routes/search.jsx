import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAddTitleToHistory } from './history.jsx';
import DisplayTitleItem from '../services/titlefunctions.jsx';
import '../App.css';

export const NL_API = 'https://www.newtlike.com:3000';
export const TMDB_API = 'https://api.themoviedb.org';
export const API_KEY = '6d931505602649b6ba683649d9af5d82';

async function searchNL(query) {
  // return empty array if no query provided
     if (!query || query.trim() === '') return [];

     // fetch initial search results (paginated list with basic info)
    const response = await fetch(encodeURI(`${NL_API}/api/titles?search=${query}`));
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    
    const titleData = await response.json();
    //const data = await response.json();
    const items = Array.isArray(titleData.items) ? titleData.items : [];
  // Fetch full details for each title 
    const dataArray = await Promise.all(items.map(async (item) => {
    const titleRes = await fetch(encodeURI(`${NL_API}/api/titles/${item.id}`));
      
      return await titleRes.json();
    }));
  // Merge item data with detailed title data
    const allData = dataArray.map((titleData, index) => {
      const item = items[index];
      return {
        ...item,
        plot: titleData.plot || 'No description available',
        rating: titleData.rating || 'N/A',
        year: titleData.year || 'N/A',
        titleType: titleData.titleType || 'N/A',
        id: titleData.id ||'no id'
      };
    });
console.log('Search results from NL:', allData);
return allData || [];

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

// Get poster from local database
async function getPosterFromNewtlike(tconst) {
  try {
    const response = await fetch(`${NL_API}/api/titles/${encodeURIComponent(tconst)}`);
    if (!response.ok) return null;

    const data = await response.json();
    return data.posterUrl || null;
  } catch (err) {
    console.error(`Error fetching poster for ${tconst}:`, err.message);
    return null;
  }
}

// Search with poster URLs
async function searchTitlePosters(query) {
  const items = await searchNL(query);
  
  const resultsWithPosters = await Promise.all(items.map(async (item) => {
    let posterUrl = null;
    
    if (item.id) {
      posterUrl = await getPosterFromNewtlike(item.id);
      
      if (!posterUrl) {
        const posterPath = await searchTMDB(item.id);
        posterUrl = posterPath ? `https://image.tmdb.org/t/p/w342${posterPath}` : null;
      }
    }
    
    return {
      ...item,
      poster_url: posterUrl
    };
  }));
  
  return resultsWithPosters;
}
 
export const maxTegn = (text, max = 220) => {
  if (text === null) return '';
  const tekst = String(text).trim();
  if (tekst.length <= max) return tekst;
  return tekst.slice(0, max).trimEnd() + ' …';
};
const Search = () => {
  const { q } = useParams();
  const [items, setItems] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const { addToHistory } = useAddTitleToHistory();

  useEffect(() => {
    if (!q || q.trim() === '') {
      setItems([]);
      return;
    }

    setIsLoading(true);
    setError(null);

    searchTitlePosters(q)
      .then(responseItems => setItems(responseItems))
      .catch(err => setError(err))
      .finally(() => setIsLoading(false));
  }, [q]);

  const handleTitleClick = async (titleId) => {
    return addToHistory(titleId);
  };

  if (isLoading) 
    return <div className='pagestuff'>Loading...</div>;
  if (error) {
    console.error('Search error:', error);
    return <div className='pagestuff'>No results found for "{q || ''}"</div>;
  }

  return (
    <div className='movieContainer'>
      {(items || []).map(movie => (
        <div className='displayMovie' key={movie.id}>
          <div className='moviePoster'>
            
            <Link to={`/title/${movie.id}`} onClick={() => handleTitleClick(movie.id)}>
            <img src={movie.poster_url} alt={movie.title} />
            </Link>
            
          </div>
          <Link to={`/title/${movie.id}`} className='textholder' onClick={() => handleTitleClick(movie.id)} >
           
            <p className='movieTitle'>{movie.title}</p>           
            <div className='movieDescription'>
              <p  >Rating: {movie.rating || 'N/A'}</p>
              <p >Year: {movie.year || 'N/A'}</p>
              <p>Type: {movie.titleType || 'N/A'}</p>
              <p className="year">{maxTegn(movie.plot)}</p>
            </div>
          
         </Link>
        </div>
      ))}
    </div>
  );
};

export default Search;