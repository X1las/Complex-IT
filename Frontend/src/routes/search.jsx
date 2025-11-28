import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import '../App.css';

const Search = () => {
  const { q } = useParams();
  const [movies, setMovies] = useState([]);
  const [loading, setLoading] = useState(true);
  const API_KEY = '6d931505602649b6ba683649d9af5d82';

  useEffect(() => {
    if (!q) return;
    
    setLoading(true);
    const url = `https://api.themoviedb.org/3/search/movie?api_key=${API_KEY}&query=${(q)}`;

    fetch(url)
      .then(response => response.json())
      .then(data => {
        console.log('Search results:', data);
        setMovies(data.results || []);
        setLoading(false);
      })
      .catch(err => {
        console.error('Search error:', err);
        setLoading(false);
      });
  }, [q]); 

  if (loading) return <div style={{padding: 20}}>Loading...</div>;
  if (!movies.length) return <div className='pagestuff'>No results found for "{(q || '')}"</div>;

  return (
    <div className='imgContainer'>
      {movies.map(movie => (
        <div className='displayMovie' key={movie.id}>
          <div className='moviePoster'>
            {movie.poster_path && (
              <img src={`https://image.tmdb.org/t/p/w342${movie.poster_path}`} alt={movie.title} />
            )}
          </div>
          <div className='textholder'>
            <p className='movieTitle'>{movie.title}</p>
            <div className='movieDescription'>
              <p>{movie.overview}</p>
              <p className="year"> From {movie.release_date ? movie.release_date.slice(0,4) : 'N/A'}</p>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default Search;