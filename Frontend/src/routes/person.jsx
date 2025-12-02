import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';

const Person = () => {
  const [personData, setPersonData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [personDataInternal, setPersonDataInternal] = useState(null);
  const [error, setError] = useState(null);
  const { id } = useParams();

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      setError(null);
      
      console.log('Fetching data for ID:', id); // Debug log
      
      try {
        // Fetch internal data
        console.log('Fetching internal data from:', `http://www.newtlike.com:3000/crew/${id}`);
        const internalRes = await fetch(`http://www.newtlike.com:3000/api/crew/${id}`);
        console.log('Internal response status:', internalRes.status);
        
        if (internalRes.ok) {
          const internalData = await internalRes.json();
          console.log('Internal data:', internalData);
          setPersonDataInternal(internalData);
        } else {
          console.log('Internal API failed with status:', internalRes.status);
        }

        // Fetch external data
        console.log('Fetching external data from TMDB...');
        const externalRes = await fetch(`https://api.themoviedb.org/3/find/${id}?external_source=imdb_id&api_key=f7cb406cd3ce48761cb8749ec2be8e89`);
        console.log('External response status:', externalRes.status);
        
        if (externalRes.ok) {
          const externalData = await externalRes.json();
          console.log('External data:', externalData);
          setPersonData(externalData.person_results?.[0] || externalData.movie_results?.[0] || null);
        } else {
          console.log('External API failed with status:', externalRes.status);
        }
        
      } catch (error) {
        console.error('Error fetching data:', error);
        setError(error.message);
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      fetchData();
    }
  }, [id]);

  if (loading) {
    return <div>Loading person data...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div className="person-container">
      <h1>Person Details - ID: {id}</h1>
      <p><strong>Current ID:</strong> {id}</p>
      
      {personDataInternal ? (
        <div>
          <h2>Internal Data</h2>
          <h3>{personDataInternal.fullname}</h3>
          <pre>{JSON.stringify(personDataInternal, null, 2)}</pre>
        </div>
      ) : (
        <p>No internal data found</p>
      )}
      
      {personData ? (
        <div>
          <h2>External Data</h2>
          <h3>{personData.name}</h3>
          <pre>{JSON.stringify(personData, null, 2)}</pre>
        </div>
      ) : (
        <p>No external data found</p>
      )}
      
      {!personDataInternal && !personData && (
        <p>No person data found for ID: {id}</p>
      )}
    </div>
  );
};

export default Person;
