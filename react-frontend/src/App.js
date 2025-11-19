import { useState, useEffect } from 'react'
import './App.css'

// Search function to fetch persons based on name, returns array of persons
function Search({ name }) {
  const [persons, setPersons] = useState([]);
  
  useEffect(() => {
    if (!name) {
      setPersons([]);
      return;
    }
    
    fetch(`https://api.themoviedb.org/3/search/person?query=${name}&api_key=f7cb406cd3ce48761cb8749ec2be8e89`)
      .then(res => res.json())
      .then(data => setPersons(data.results || []))
      .catch(error => {
        console.error('Error fetching data:', error);
        setPersons([]);
      });
  }, [name]); 
  console.log("Searched for:", name, "Found persons:", persons);
  return persons;
}

// Component to display details of a person, uses KnownFor component
function DisplayPerson({ person }) {
  if (!person) return <div>No person to display</div>;

  return (
    <div>
      <h2>{person.name}</h2>
      <p>Known for: </p>
      <KnownFor person={person} />
      <p>Popularity: {person.popularity}</p>
      {person.profile_path && (
        <img
          src={`https://image.tmdb.org/t/p/w200${person.profile_path}`} 
          alt={person.name}
          style={{ width: '100px', height: '150px', objectFit: 'cover' }}
        />
      )}
      {console.log("Person Displayed:", person)}
    </div>
  );
}

// Component to generate list of known works of a person
function KnownFor({ person }) {
  if (!person || !person.known_for) return <div>No known works to display</div>;

  return (
    <div>
      <h3>Known For:</h3>
      <ul>
        {person.known_for.map((work) => (
          <li key={work.id}>
            {work.title || work.name}
          </li>
        ))}
        {console.log("Known works:", person.known_for)}
      </ul>
    </div>
  );
}

function App({ name }) {
  const [searchName, setSearchName] = useState(name || '');
  const [counter, setCounter] = useState(0);
  const persons = Search({ name: searchName });

  return (
    <div>
      {/* Search Field, updates searchName state on change */}
      <input
        type="text"
        value={searchName}
        onChange={(e) => setSearchName(e.target.value)}
        placeholder="Search for a person"
      />

      <h1>Showing search result {counter + 1} of {persons.length} for {searchName}:</h1>
      
      {/* Navigation Buttons update counter */}
      <button 
        onClick={() => setCounter((prev) => Math.max(prev - 1, 0))} 
        disabled={counter === 0} 
      >
        Previous
      </button>
      <button 
        onClick={() => setCounter((prev) => Math.min(prev + 1, persons.length - 1))} 
        disabled={counter >= persons.length - 1}
      >
        Next
      </button>

      {/* Displays person or no results */}
      {persons.length > 0 ? (
        <DisplayPerson person={persons[counter]} />
      ) : (
        <p>No results found</p>
      )}
    </div>
  );
}

export default App