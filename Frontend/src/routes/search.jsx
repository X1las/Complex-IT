import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAddTitleToHistory } from './history.jsx';
import DisplayTitleItem from '../services/titlefunctions.jsx';
import '../App.css';

export const NL_API = 'https://www.newtlike.com:3000';
export const TMDB_API = 'https://api.themoviedb.org';
export const API_KEY = '6d931505602649b6ba683649d9af5d82';

// Search local database
async function searchNL(query) {
  if (!query || query.trim() === '') return [];

  const response = await fetch(`${NL_API}/api/titles?search=${encodeURIComponent(query)}`);
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
  
  const data = await response.json();
  return data.items || [];
}

// TMDB fallback for posters
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

// Main Search component
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
    return <div style={{ padding: 20 }}>Loading...</div>;
  if (error) {
    console.error('Search error:', error);
    return <div className='pagestuff'>No results found for "{q || ''}"</div>;
  }

  return (
    <div className='search-results-container' style={{ 
      padding: '20px', 
      backgroundColor: '#1a1a1a', 
      minHeight: '100vh' 
    }}>
      <h2 style={{ color: 'white', marginBottom: '20px' }}>
        Search Results for "{q}"
      </h2>
      {(items || []).length === 0 ? (
        <div style={{ color: 'white' }}>No results found</div>
      ) : (
        items.map(movie => (
          <DisplayTitleItem key={movie.id} tconst={movie.id} />
        ))
      )}
    </div>
  );
};

export default Search;