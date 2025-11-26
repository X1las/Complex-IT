import { useState, useEffect } from 'react'
import './App.css'

function KnownFor({ knownforlist }) {
  return (
    <div>
      <h3>Known for:</h3>
      <div className="smallknown">
        {knownforlist.map(item => (
          <div key={item.id}>
            <p className="item-title">{item.title || item.name} </p><p> ({item.release_date?.slice(0,4) || item.first_air_date?.slice(0,4)})</p>
            <div className="smallimg-container">
            <p className="overview">{item.overview}</p>
            {item.poster_path && (<img className="smallimg" src={`http://image.tmdb.org/t/p/w200${item.poster_path}`}/>)}
          </div></div>
        ))}
      </div>
    </div>
  );
}

// måske lav den på en anden måde forstår ikke atm

function ActorsMovies({ movieId }) {
  const [cast, setCast] = useState([]);
  const API_KEY = '6d931505602649b6ba683649d9af5d82';

  useEffect(() => {
    if (!movieId) return;
    
    const url = `https://api.themoviedb.org/3/movie/${movieId}/credits?api_key=${API_KEY}`;
    
    fetch(url)
      .then(response => response.json())
      .then(data => {
        console.log('Cast:', data.cast);
        setCast(data.cast || []);
      })
      .catch(error => console.error('Error:', error));
  }, [movieId]);

  return (
    <div>
      <h3>Cast:</h3>
      <div className="smallknown">
        {cast.map(actor => (
          <div key={actor.id}>
            <p>{actor.name} as {actor.character}</p>
            {actor.profile_path && (
              <img 
                className="smallimg" 
                src={`http://image.tmdb.org/t/p/w200${actor.profile_path}`} 
                alt={actor.name} 
              />
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

function App() {

  const [movies, setMovies] = useState([]);
  const [persons, setPerson] = useState([]);
  const [search, setSearch] = useState('');
  const [searchp, setSearchp] = useState('');

  const [count, setCount] = useState(0);
  const [countm, setCountm] = useState(0);


  const [img, setImg] = useState('');

  const API_KEY = '6d931505602649b6ba683649d9af5d82';  

  useEffect(() => {
    const q = search;
    const url = `https://api.themoviedb.org/3/search/movie?api_key=${API_KEY}&query=${q}`

    fetch(url)
      .then(response => response.json())
      .then(data => {
        console.log('TMDB response:', data)
        setMovies(data.results || [])
        setCountm(0)
      })
      
  }, [search]);


  useEffect(() => {
    const q = searchp;
    const url = `https://api.themoviedb.org/3/search/person?api_key=${API_KEY}&query=${q}`

    fetch(url)
      .then(response => response.json())
      .then(data => {
        console.log('TMDB response:', data)
        setPerson(data.results || [])
        setCount(0) 
      }
    )
  }, [searchp]);

  useEffect(() => {
        // GET request using fetch inside useEffect React hook
fetch(`https://api.themoviedb.org/3/collection?api_key=${API_KEY}`)
            .then(response => response.json())
            .then(data => {
            (console.log('TMDB response:', data))
            setImg(data.results || [])
            })
          }, []);

  return (
    <>
       <div className="container">
  
     <div>
     
      <button onClick={() => setCountm((prev) => Math.max(prev - 1, 0))} disabled={countm === 0}>
        Previous
      </button>
      <button onClick={() => setCountm((prev) => Math.min(prev + 1, movies.length - 1))} disabled={countm >= movies.length - 1 || movies.length === 0}>
        Next
      </button>

      <input id="searchbar" placeholder="Search movies" onChange={e => setSearch(e.target.value)}/>

      <input id="searchbarperson" placeholder="Search person" onChange={e => setSearchp(e.target.value)}/>

      <button onClick={() => setCount((prev) => Math.max(prev - 1, 0))} disabled={count === 0}>
        Previous
      </button>
      <button onClick={() => setCount((prev) => Math.min(prev + 1, persons.length - 1))} disabled={count >= persons.length - 1 || persons.length === 0}>
        Next
      </button>

     </div>

    <div className="show"> 

  {movies.length > 0 && (
    <div className="movies" key={movies[countm].id}>
      <h2>{movies[countm].title}</h2>
      <p>ID: {movies[countm].id}</p>
      {movies[countm].poster_path && (<img src={`http://image.tmdb.org/t/p/original${movies[countm].poster_path}`}  />)} 
       {/* if statment x ? if true this : if false that */}
      <p>From {movies[countm].release_date ? movies[countm].release_date.slice(0,4) : "N/A"}</p>
      <p>{movies[countm].overview}</p>
      <p>Movie {countm + 1} af {movies.length}</p>
      <ActorsMovies movieId={movies[countm].id} />
        </div>
  )}

{/* <ShowPersons persons={persons} count={count} /> */}

   {persons.length > 0 && (
    <div className="persons" key={persons[count].id}>
      <h2>{persons[count].original_name}</h2>
      <p>ID: {persons[count].id}</p>
      {persons[count].profile_path && (<img src={`http://image.tmdb.org/t/p/original${persons[count].profile_path}`} />)} 
      <p>{persons[count].known_for_department+"   Popularity: "}{persons[count].popularity}</p>
      <p>Person {count + 1} af {persons.length}</p>
      {persons[count].known_for && persons[count].known_for.length > 0 && (
        <KnownFor knownforlist={persons[count].known_for} />
      )}
          </div>
  )}

      </div>
    </div>
    </>
  )
}
function ShowPersons(persons,count){
  if(persons.length === 0) return null;

  return (
    <div className="persons" key={persons[count].id}>
      <h2>{persons[count].original_name}</h2>
      <p>ID: {persons[count].id}</p>
      {persons[count].profile_path && (<img src={`http://image.tmdb.org/t/p/original${persons[count].profile_path}`} />)} 
      <p>{persons[count].known_for_department+"   Popularity: "}{persons[count].popularity}</p>
      <p>Person {count + 1} af {persons.length}</p>
      {persons[count].known_for && persons[count].known_for.length > 0 && (
        <KnownFor knownforlist={persons[count].known_for} />
      )}
    </div>
  );
}
export default App
