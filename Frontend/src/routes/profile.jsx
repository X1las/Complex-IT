import React, { useState, useEffect, use } from 'react';
import { useParams } from 'react-router-dom';
import '../css/profile.css';
import { Link, Outlet } from 'react-router-dom';

const Profile = () => {
    const { id } = useParams();
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
/* const response = await fetch(`${API_URL}/api/users/{username} */
      setProfile({
        id: id,
        username: id || 'John',
      });
      setLoading(false);
    }, [id]);



  if (loading) return <div style={{padding: 20}}>Loading profile...</div>;
  if (!profile) return <div style={{padding: 20}}>User not found</div>;

    return (
    <div className='profile'>
      <div className='top'>
        <h1>{profile.username}'s Page</h1>
      </div>
      
      <div>
          <div className='buttonContainer'>
            <button className='btnProfile'><Link to={`/profile/${id}/bookmarks`} >Bookmarks</Link></button>
            <button className='btnProfile'><Link to={`/profile/${id}/history`} >History</Link></button>
            <button className='btnProfile'><Link to={`/profile/${id}/ratings`}>Ratings</Link></button>
          </div>
        </div>
      <Outlet />
    </div>
  );
};

export default Profile;
