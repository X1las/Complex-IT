// import { useState, useEffect } from 'react'
// import './home.css'
// import logo from './assets/image.png';
// import icon from './assets/icon.png'; 
// import ReactDOM from "react-dom/client";
// import { BrowserRouter, Routes, Route } from "react-router";



// function Home() {

//   const [search, setSearch] = useState('star wars');
//   const [movies, setMovies] = useState([]);

//   const API_KEY = '6d931505602649b6ba683649d9af5d82';  

//   useEffect(() => {
//      if (!search) return; 
    
//     const url = `https://api.themoviedb.org/3/search/movie?api_key=${API_KEY}&query=${search}`

//     fetch(url)
//       .then(response => response.json())
//       .then(data => {
//         console.log('TMDB response:', data)
//         setMovies(data.results || [])
//       })
      
//   }, [search]);

//   return (
//     <>
//         <nav>
//             <h1>test</h1>
//             {/* <img src="" alt="" /> */}
//             <div >< img className='imgPlaceholder' src={logo} alt="Logo" /></div>
//             <input className='searchField' type="searchField" onChange={e => setSearch(e.target.value)} />
//             <div>< img className='profileplaceholder' src={icon} alt="Logo" /></div>
            
//         </nav>
//             <div className='imgContainer'>
//                     {/* skal loopes for dem alle */}
//                     {movies.length > 0 && movies.slice(0,10).map(movie => (
//                 <div className='displayMovie' key={movie.id}>
//                     <div className='moviePoster'>
//                       {movie.poster_path && (
//                         <img src={`https://image.tmdb.org/t/p/w342${movie.poster_path}`} alt={movie.title} />
//                       )}
//                     </div>
//                     <div className='textholder'>
//                       <p className='movieTitle'>{movie.title}</p>
//                       <div className='movieDescription'><p>{movie.overview}</p>
//                       <p className="year"> From {movie.release_date ? movie.release_date.slice(0,4) : 'N/A'}</p></div>
//                     </div>
//                 </div>
//                  ))}
//             </div>
//     </>
//   )
// }

// export default Home

