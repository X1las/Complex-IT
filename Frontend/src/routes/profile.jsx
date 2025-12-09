import { useParams } from 'react-router-dom';
import '../css/profile.css';
import { Link, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';

const Profile = () => {
    const { id } = useParams();
    const { user } = useAuth();
    const { logout } = useAuth();
    const handleLogout = () => {logout();};

  if (!user) return <Link to={`/`}><div style={{padding: 20}}>User not found</div></Link>;

    return (
    <div className='profile'>
      <div className='top'>
        <h1>{user.username}'s Page</h1>
      </div>
      
      <div className='buttonContainer'>
        <button className='btnProfile'><Link to={`/profile/${id}/bookmarks`}>Bookmarks</Link></button>
        <button className='btnProfile'><Link to={`/profile/${id}/history`}>History</Link></button>
        <button className='btnProfile'><Link to={`/profile/${id}/ratings`}>Ratings</Link></button>
        <button className='btnProfile' onClick={handleLogout}> <Link to={`/`}>Logout</Link></button>
      </div>

      <div className='contentContainer'>
        <div className="outlet"><Outlet/></div>
      </div>
    </div>
  );
};

export default Profile;