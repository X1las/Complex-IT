import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import '../css/person.css';
import '../css/general.css';

const KnownForItem = ({ item }) => {
  return (
    <div className="known-for-item">
      <h3>{item.title || item.name}</h3>
      <img src={
        item.poster_path
          ? `https://image.tmdb.org/t/p/w200${item.poster_path}`
          : null
      } alt="" />
    </div>
  );
}

const FindPersonDetails = async (personId) => {
  const response = await fetch(`https://api.themoviedb.org/3/person/${personId}?api_key=f7cb406cd3ce48761cb8749ec2be8e89`);
  if (response.ok) {
    const data = await response.json();
    return data;
  } else {
    throw new Error('Failed to fetch person details');
  }
}

const FindPersonInternal = async (imdbId) => {
  const response = await fetch(`https://www.newtlike.com:3000/api/crew/${imdbId}`); // Changed to HTTPS
  if (response.ok) {
    const data = await response.json();
    return data;
  } else {
    throw new Error('Failed to fetch internal person details');
  }
}

const FindPersonExternal = async (imdbId) => {
  const response = await fetch(`https://api.themoviedb.org/3/find/${imdbId}?external_source=imdb_id&api_key=f7cb406cd3ce48761cb8749ec2be8e89`);
  if (response.ok) {
    const data = await response.json();
    return data;
  } else {
    throw new Error('Failed to fetch external person details');
  }
}

const FindPersonMerged = async (imdbId) => {
  

  // Using FindPersonInternal and FindPersonExternal to get merged data
  const internalData = await FindPersonInternal(imdbId);
  const externalData = await FindPersonExternal(imdbId);

  if (externalData?.person_results?.length > 0) {
    const personDetails = await FindPersonDetails(externalData.person_results[0].id);
    return { ...internalData, ...externalData, personDetails };
  }
  else {
    return { ...internalData, ...externalData };
  }
}

const Person = () => {
  const [personData, setPersonData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { id } = useParams();

  useEffect(() => {
    const fetchPersonData = async () => {
      try {
        setLoading(true);
        setError(null);
        
        console.log('Fetching data for ID:', id); // Debug log
        const mergedData = await FindPersonMerged(id);
        console.log('Merged data:', mergedData); // Debug log
        
        setPersonData(mergedData);
      } catch (err) {
        console.error('Error fetching person data:', err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      fetchPersonData();
    }
  }, [id]);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!personData) return <div>No person data found</div>;

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
          <h1 className="poster-title">{personData?.name || personData?.fullname}</h1>
          <p> <strong>{"Department: " + (personData?.known_for_department || "Unknown Department")}</strong></p>
          <p>{"Birthdate: " + (personData?.birthdate || "Unknown Birthdate")}</p>
          <p>{"Place of Birth: " + (personData?.place_of_birth || "Unknown Place of Birth")}</p>
          <p>{"Death: " + (personData?.deathdate || "Unknown Deathdate")}</p>
          <p>{"Age: " + (personData?.age || "Unknown Age")}</p>
          <p className='poster-description'>{personData?.biography || "No Biography Available"}</p>
        </div>
      </div>
      <div className="known-for">
        <h2>Known For</h2>
        <ul>
          {personData?.person_results?.[0]?.known_for?.map((item) => (
            <KnownForItem key={item.id} item={item} />
          )) || null}
        </ul>
      </div>
    </div>
  );
};

export default Person;
export { FindPersonMerged };
