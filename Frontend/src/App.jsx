import { useState, useEffect } from 'react'
import './App.css'
import logo from './assets/image.png';
import icon from './assets/icon.png'; 
import { Link, Outlet, useNavigate } from 'react-router-dom';



function App() {

  const [search, setSearch] = useState('');
  const navigate = useNavigate();

  const onSearchSubmit = (e) => {
    e.preventDefault();
    if (!search.trim()) return;
    navigate(`/search/${encodeURIComponent(search)}`);
  }

  return (
    <>
        <nav>
            <div ><Link to="/"><img className='imgPlaceholder' src={logo} alt="Logo" /></Link></div>
            <form onSubmit={onSearchSubmit} style={{display:'inline'}}> 
              <input className='searchField' type="text" value={search} onChange={e => setSearch(e.target.value)} />
            </form>
            <div><Link to="/profile/John"><img className='profileplaceholder' src={icon} alt="profilePic" /></Link></div>
          <Link to="/">Home</Link> | 
          <Link to="/search">Search</Link> | 
          {user ? (
            <>
              <span>Welcome, {user.username || user.Username}!</span> | 
              <Link to={`/profile/${user.id || user.username}`}>Profile</Link> | 
              <Link to={`/profile/${user.id || user.username}/ratings`}>Ratings</Link> | 
              <button onClick={handleLogout} style={{ cursor: 'pointer', background: 'none', border: 'none', color: 'inherit', textDecoration: 'underline' }}>Logout</button>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link> | 
              <Link to="/register">Register</Link>
            </>
          )}
        </nav>
        <Outlet />
    </>
  )
}

export default App
