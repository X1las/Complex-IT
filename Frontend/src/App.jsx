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
            <div >< img className='imgPlaceholder' src={logo} alt="Logo" /></div>
            <form onSubmit={onSearchSubmit} style={{display:'inline'}}> 
              <input className='searchField' type="text" value={search} onChange={e => setSearch(e.target.value)} />
            </form>
            <div><Link to="/profile/:id/"><img className='profileplaceholder' src={icon} alt="Logo" /></Link></div>
        </nav>
        <Outlet />
    </>
  )
}

export default App
