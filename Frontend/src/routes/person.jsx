import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import '../css/person.css';
import '../css/general.css';
import { NL_API,TMDB_API,API_KEY } from './search';
import DisplayTitleItem from '../services/titleDisplayItem';

// Comprehensive function to get all actor/person data from both internal API and TMDB
const getPerson = async ( nconst ) => {

  console.log('Fetching comprehensive person data for:', nconst);

  try {
    const internalData = fetch(`${NL_API}/api/crew/${nconst}`).then(response => {
      if (!response.ok) throw new Error(`Internal person API failed: ${response.status}`);
      return response.json();
    });

    const externalData = fetch(`${TMDB_API}/3/find/${nconst}?external_source=imdb_id&api_key=${API_KEY}`).then(response => {
      if (!response.ok) throw new Error(`TMDB find API failed: ${response.status}`);
      return response.json();
    });

    const knownFor = fetch(`${NL_API}/api/crew/${nconst}/titles`).then(response => {
      if (!response.ok) throw new Error(`Internal person API failed: ${response.status}`);
      return response.json();
    });

    const mergedData = await Promise.all([internalData, externalData, knownFor]).then(([internal, external, knownFor]) => {
      const tmdbPerson = external.person_results?.[0] || null;
      return {
        ...internal,
        ...tmdbPerson,
        knownFor
      };
    });

    console.log('Merged person data:', mergedData);
    
    return mergedData;
  } catch (error) {
    console.error('Error in comprehensive person fetch:', error);
    return null;
  }
};

const Person = () => {
  const [personData, setPersonData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { id } = useParams();

  useEffect(() => {
    const fetchData = async () => {
      if (!id) return;
      
      setLoading(true);
      setError(null);
      console.log('Fetching comprehensive actor data for ID:', id);

      try {
        const data = await getPerson(id);
        setPersonData(data);
        
        if (!data) {
          setError('No person data found');
        }
        
      } catch (error) {
        console.error('Error fetching actor data:', error);
        setError(error.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  if (loading) {
    return <div>Loading person data...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div className="person-container">
      <div className="poster-box">
        <img className="poster-image"
          src={
            personData?.profile_path
              ? `https://image.tmdb.org/t/p/w200${personData.profile_path}`
              : null
          }
          alt="No Image Available" />
        <div className="poster-content-box">
          <h1 className="poster-title">{personData?.name}</h1>
          <p className='poster-description'>{personData?.biography}</p>
          {personData?.birthYear && (
            <p className="birth-year">Born: {personData.birthYear}</p>
          )}
          {personData?.deathYear && (
            <p className="death-year">Died: {personData.deathYear}</p>
          )}
          {personData?.known_for_department && (
            <p className="known-for-department">Known for: {personData.known_for_department}</p>
          )}
        </div>
      </div>
      <div className="known-for">
        <h2>Known For</h2>
        <div className="known-for-container">
          {/* Display internal known for titles */}
          {personData?.knownFor?.items.map((item, index) => (
            <DisplayTitleItem key={`internal-${item.titleId || index}`} tconst={item.titleId} />
          ))}
          
          {/* Show message if no data available */}
          {(!personData?.knownFor?.length && !personData?.knownForTitles?.length) && (
            <div>No known for titles available</div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Person;