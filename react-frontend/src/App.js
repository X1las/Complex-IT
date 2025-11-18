import { useState } from 'react'
import { useEffect } from 'react'
import './App.css'

function App({ count }) {
  const [users, setUsers] = useState([]);
  useEffect(() => {
    fetch("https://api.themoviedb.org/3/search/person?query=anna&api_key=f7cb406cd3ce48761cb8749ec2be8e89")
      .then(res => res.json())
      .then(data => setUsers(data.results));
  }, [count]);
  console.log(users);
  return (
    <div>
      {users.map((u, index) => (
        <div key={index}>
          <img src={`https://image.tmdb.org/t/p/original${u.profile_path}`}/>
          <p>{u.name} , {u.id}</p>
        </div>
      ))}
    </div>
  );
}


export default App