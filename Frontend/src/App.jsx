import { useState, useEffect } from 'react'
import './App.css'
import logo from './assets/image.png';
import icon from './assets/icon.png'; 
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext.jsx';



function App() {

  const [search, setSearch] = useState('');
  const navigate = useNavigate();
  const {user} = useAuth();

  /* console.log('Current user in App component:', user); */
  const onSearchSubmit = (e) => {
    e.preventDefault();
    if (!search.trim()) return;
    navigate(`/search/${(search)}`);
  }

  return (
    <>
        <nav>
            <div ><Link to="/"><img className='imgPlaceholder' src={logo} alt="Logo" /></Link></div>
            <form onSubmit={onSearchSubmit} style={{display:'inline'}}> 
              <input className='searchField' type="text" value={search} onChange={e => setSearch(e.target.value)} />
            </form>
            <div><Link to={user ? `/profile/${user.username}` : `/login`}> <img className='profileplaceholder' src={icon} alt="profilePic" /></Link></div>
        </nav>
        <Outlet />
    </>
  )
}

export default App
