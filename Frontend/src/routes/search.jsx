import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import '../App.css';

// Fetch poster New
async function getPosterFromNewtlike(tconst) {
  try {
    const url = `/api/titles/${(tconst)}`;
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

async function searchNL(query) {
  const pagin = 'page=1&pageSize=10'
  const url = `/api/titles?search=${(query)}&${pagin}`;

  const response = await fetch(url);
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
    
  const data = await response.json();
  
  const items = data.items || [];
  const filteredItems = items.filter(item => 
    ['movie', 'tvSeries', 'tvMovie', 'short', 'tvShort'].includes(item.titleType)
  );
  
  return filteredItems;
}
// TMDB fallback
async function searchTMDB(tconst) {
  const API_KEY = '6d931505602649b6ba683649d9af5d82';
  const url = `https://api.themoviedb.org/3/find/${tconst}?external_source=imdb_id&api_key=${API_KEY}`;

  const response = await fetch(url);
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
    
  const data = await response.json();
  const movie = data.movie_results?.[0];
  const tv = data.tv_results?.[0];
  
  return (movie?.poster_path || tv?.poster_path) || null;
}

async function searchTitlePosters(query) {
  const items = await searchNL(query);
  
  let dbCount = 0;
  let tmdbCount = 0;
  let noneCount = 0;
  
  const resultsWithPosters = await Promise.all(items.map(async (item) => {
    let posterUrl = null;
    
    if (item.id) {
      posterUrl = await getPosterFromNewtlike(item.id);
      

      if (posterUrl) {
        
        dbCount++;
        console.log(`[NEWTLIKE] ${item.title} (${item.id})`);
      } 
      //Fallback til TMDB hvis ingen poster i database
      else {
        const posterPath = await searchTMDB(item.id);
        posterUrl = posterPath ? `https://image.tmdb.org/t/p/w342${posterPath}` : null;
        
        if (posterUrl) {
          tmdbCount++;
          console.log(`[TMDB] ${item.title} (${item.id})`);
        } else {
          noneCount++;
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
  const [movies, setMovies] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!q) return;
    let mounted = true;

    (async () => {
      try {
        setLoading(true);
        const results = await searchTitlePosters(q);
        if (mounted) setMovies(results);
      } catch (err) {
        console.error('Search error:', err);
        if (mounted) setMovies([]);
      } finally {
        if (mounted) setLoading(false);
      }
    })();

    return () => { mounted = false; };
  }, [q]);

  if (loading) return <div style={{padding: 20}}>Loading...</div>;
  if (!movies.length) return <div className='pagestuff'>No results found for "{q || ''}"</div>;

  return (
    <div className='imgContainer'>
      {movies.map(movie => (
        <div className='displayMovie' key={movie.id}>
          <div className='moviePoster'>
            {movie.poster_url ? (
              <img src={movie.poster_url} alt={movie.title} />
            ) : (
              <div className='noImage'></div>
            )}
          </div>
          <div className='textholder'>
            <p className='movieTitle'>{movie.title}</p>
            <div className='movieDescription'>
              <p>Rating: {movie.rating || 'N/A'}</p>
              <p className="year">Year: {movie.year || 'N/A'}</p>
              <p>Type: {movie.titleType || 'N/A'}</p>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default Search;