import { useState } from 'react'
import { useEffect } from 'react'
import './App.css'

function Search({name}) {
  const [users, setUsers] = useState([]);
  useEffect(() => {
    if (!name) {
      setUsers([]);
      return;
    }
    
    fetch(`https://api.themoviedb.org/3/search/person?query=${name}&api_key=f7cb406cd3ce48761cb8749ec2be8e89`)
      .then(res => res.json())
      .then(data => setUsers(data.results || []))
      .catch(error => {
        console.error('Error fetching data:', error);
        setUsers([]);
      });
  }, [name]); 
  return users;
}

function DisplayPerson({ person }) {
  if (!person) return <div>No person to display</div>;

  return (
    <div style={{marginLeft: '30%', marginRight: '30%', backgroundColor: '#c2c1c1ff', padding: '1%', borderRadius: '8px'}}>
      <h2>{person.name}</h2>
      <p>Known for: {person.known_for_department}</p>
      <p>Popularity: {person.popularity}</p>
      <img
        src={`https://image.tmdb.org/t/p/original${person.profile_path}`} 
        alt={person.name}
        style={{width: '70%', height: 'auto', objectFit: 'cover'}}
      />
    </div>
  );
}

function App({ name }) {
  const [username, setUsername] = useState(name || '');
  const [counter, setCounter] = useState(0);
  const users = Search({ name: username });
  console.log(users);


  return (
    <div style={{textAlign: 'center', paddingLeft: '10%', paddingRight: '10%', backgroundColor: '#f0f0f0', paddingBottom: '20%', borderRadius: '10px'}}>
      <input 
        type="text"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        placeholder="Search for a person"
      />
      <h1> Showing search result {counter+1} of {users.length} for {username}:</h1>
      <button onClick={() => setCounter((prev) => Math.max(prev - 1, 0))} disabled={counter === 0}>
        Previous
      </button>
      <button onClick={() => setCounter((prev) => Math.min(prev + 1, users.length - 1))} disabled={counter >= users.length - 1}>
        Next
      </button>
      {users.length > 0 ? (
        <DisplayPerson person={users[counter]} />
      ) : (
        <p>No results found</p>
      )}
    </div>
  );
}

export default App