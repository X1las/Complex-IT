import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import '../css/person.css';
import '../css/general.css';
import { NL_API,TMDB_API,API_KEY } from './search';

const KnownForItem = ({ item, profilePath }) => {
  return (
    <div className="known-for-item">
      <Link to={`/title/${item.id}`} state={{ profilePath }}>
        <h3>{item.title || item.name}</h3>
        <img src={ item.poster_path ? `https://image.tmdb.org/t/p/w200${item.poster_path}` : null } 
        alt="" />
      </Link>
    </div>
  );
}

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
        console.log('Fetching internal data from:', `${NL_API}/crew/${id}`);
        const internalRes = await fetch(`${NL_API}/api/crew/${id}`);
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
        const externalRes = await fetch(`${TMDB_API}/3/find/${id}?external_source=imdb_id&api_key=${API_KEY}`);
        console.log('External response status:', externalRes.status);

        if (externalRes.ok) {
          const externalData = await externalRes.json();
          console.log('External data:', externalData);
          setPersonData(externalData.person_results?.[0] || externalData.movie_results?.[0] || null);

          // fetch person details if person_results is available
          if (externalData.person_results?.length > 0) {
            const personId = externalData.person_results[0].id;
            console.log('Fetching person details for ID:', personId);
            const personDetailsRes = await fetch(`${TMDB_API}/3/person/${personId}?api_key=${API_KEY}`);
            console.log('Person details response status:', personDetailsRes.status);

            if (personDetailsRes.ok) {
              const personDetailsData = await personDetailsRes.json();
              console.log('Person details data:', personDetailsData);
              const personCombinedData = { ...personDetailsData, ...externalData.person_results[0] };
              setPersonData(personCombinedData);
            } else {
              console.log('Person details API failed with status:', personDetailsRes.status);
            }
          }
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
      <div className="poster-box">
        <img className="poster-image"
          src={
            personData?.profile_path
              ? `https://image.tmdb.org/t/p/w200${personData.profile_path}`
              : null
          }
          alt="No Image Available" />
        <div className="poster-content-box">
          <h1 className="poster-title">{personData?.name || personDataInternal?.fullname}</h1>
          <p className='poster-description'>{personData?.biography || personDataInternal?.biography}</p>
        </div>
      </div>
      <div className="known-for">
        <h2>Known For</h2>
        <ul>
          {personData?.known_for?.map((item) => (
            <KnownForItem key={item.id} item={item} profilePath={personData?.profile_path}/>
          )) || null}
        </ul>
      </div>
    </div>
  );
};

export default Person;