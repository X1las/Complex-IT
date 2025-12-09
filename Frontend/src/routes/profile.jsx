import { useParams } from 'react-router-dom';
import '../css/profile.css';
import { Link, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';

const Profile = () => {
    const { id } = useParams();
    const { user, logout } = useAuth();

    const handleLogout = () => {
        logout();
    };

    if (!user) return <Link to={`/`}><div style={{padding: 20}}>User not found</div></Link>;

    return (
        <div className='profile'>
            <div className='top'>
                <h1>{user.username}'s Page</h1>
            </div>
            
            <div className='buttonContainer'>
                <Link to={`/profile/${id}/bookmarks`} className='btnProfile'>
                    Bookmarks
                </Link>
                <Link to={`/profile/${id}/history`} className='btnProfile'>
                    History
                </Link>
                <Link to={`/profile/${id}/ratings`} className='btnProfile'>
                    Ratings
                </Link>
                <Link to={`/`} className='btnProfile' onClick={handleLogout}>
                    Logout
                </Link>
            </div>

            <div className='contentContainer'>
                <div className="outlet"><Outlet/></div>
            </div>
        </div>
    );
};

export default Profile;