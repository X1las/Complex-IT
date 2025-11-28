import { useState } from 'react'
import { Outlet, Link } from 'react-router-dom'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
    <div>
      <header>
        <h1>Complex-IT Application</h1>
        <nav>
          <Link to="/">Home</Link> | 
          <Link to="/search">Search</Link> | 
          <Link to="/login">Login</Link> | 
          <Link to="/profile/1">Profile</Link>
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
