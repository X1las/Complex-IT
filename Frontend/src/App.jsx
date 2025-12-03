import { Outlet, Link, useNavigate } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import './App.css'

function App() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div>
      <header>
        <h1>Complex-IT Application</h1>
        <nav>
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
      </header>
      
      <main>
        <p>Welcome to Complex-IT Application Frontend!</p>
        {/* This is where child routes will be rendered */}
        <Outlet />
      </main>
    </div>
  )
}

export default App
