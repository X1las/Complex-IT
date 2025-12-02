import { useState, useEffect } from 'react';
import { useParams } from 'react-router';



const Person = () => {
  const [personData, setPersonData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [personDataInternal, setPersonDataInternal] = useState([]);
  const {id} = useParams();

  setLoading(true);

  fetch('http://newtlike.com:3000/crew/'+id)
      .then(res => res.json())
      .then(data => setPersonDataInternal(data))
      .catch(error => {
        console.error('Error fetching data:', error);
        setPersonDataInternal([]);
    });

  fetch(`https://api.themoviedb.org/3/find/${id}?external_source=imdb_id&api_key=f7cb406cd3ce48761cb8749ec2be8e89`)
    .then(res => res.json())
    .then(data => setPersonData(data.results || []))
    .catch(error => {
      console.error('Error fetching data:', error);
      setPersonData([]);
  });

  setLoading(false);

  useEffect(() => {
    // TODO: Load person data on component mount
  }, []);

  return (
    <div className="person-container">
      <h1>{personDataInternal.fullname || personData.name}</h1>
      {/* TODO: Add person details UI */}
    </div>
  );
};

export default Person;
