import { useState } from 'react'
import { useEffect } from 'react'
import './App.css'

function App({ count }) {
  const [users, setUsers] = useState([]);
  useEffect(() => {
    fetch("https://api.themoviedb.org/3/search/person?query=david&api_key=f7cb406cd3ce48761cb8749ec2be8e89")
      .then(res => res.json())
      .then(data => setUsers(data.results));
  }, [count]);
  return (
    <div>
      {users.map((u, index) => (
        <p key={index}>{u.id}</p>
      ))}
    </div>
  );
}


export default App