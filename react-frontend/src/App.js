// Group 3 Members: Jonas Wolffrom Strandgaard Clausen, Malthe Tranberg Ørsted, Joakim Dorph Broager

import { useState, useEffect } from 'react'
import './App.css'

// Search function to fetch persons based on name, returns person array
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
  }, [name]);
  console.log("Searched for:", name, "Found persons:", persons);
  return persons;
}

// Component to display details of a person, uses KnownFor component
function DisplayPerson({ person }) {
  // Out of bounds fix
  if (!person) return <div>No person to display</div>;

  return (
    <div style={{ marginLeft: '30%', marginRight: '30%', backgroundColor: '#c2c1c1ff', padding: '1%', borderRadius: '8px' }}>
      <h2>{person.name}</h2>
      {person.profile_path && (
        <img
          src={`https://image.tmdb.org/t/p/w200${person.profile_path}`}
          alt={person.name}
          style={{ width: '50%', height: 'auto', objectFit: 'cover' }}
        />
      )}
      <p>Profession: {person.known_for_department}</p>
      {/* KnownFor component rendering */}
      <KnownFor person={person} />
      <p>Popularity: {person.popularity}</p>

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
        {person.known_for.map((work) => (
          <div key={work.id}>
            <strong>{work.title}</strong><br></br>
            <img src={`https://image.tmdb.org/t/p/w200${work.poster_path}`} alt={work.title} /><br></br>
            <em>Media Type: {work.media_type}</em><br></br>
            <em>Release Date: {work.release_date}</em><br></br>
            <p>{work.overview}</p>
          </div>
        ))}
        {console.log("Known works:", person.known_for)}
    </div>
  );
}

function App({ name }) {
  const [searchName, setSearchName] = useState(name || '');
  const [counter, setCounter] = useState(0);
  const persons = Search({ name: searchName });

  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
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