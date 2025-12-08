import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import '../css/profile.css';
import { Link, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import {useAddBookmarks} from './bookmarks.jsx';
import {useBookmarks} from './bookmarks.jsx';
import {useRemoveBookmark} from './bookmarks.jsx';
/* 
testprofile
ruccer123
 */
function useUser() {
  const {user} = useAuth();
  const username = user ? user.username : null;
  return username;
}
/* function useRemoveBookmarkFromUser(titleId) {
  const {removeBookmark} = useRemoveBookmark(titleId);
  removeBookmark(titleId);
} */
const Profile = () => {
    const { id } = useParams();
    //const [loading, setLoading] = useState(false);
    const user = useUser();
    const { bookmarks, error } = useBookmarks();
    const { logout } = useAuth();
    const handleLogout = () => {logout();};

    const { removeBookmark } = useRemoveBookmark();

  //if (loading) return <div style={{padding: 20}}>Loading profile...</div>;
  if (!user) return <Link to={`/`}>logout<div style={{padding: 20}}>User not found</div></Link>;

    return (
    <div className='profile'>
      <div className='top'>
        <h1>{user}'s Page</h1>
      </div>
      
      <div>
          <div className='buttonContainer'>
            <button className='btnProfile'><Link to={`/profile/${id}/history`}>History</Link></button>
            <button className='btnProfile'><Link to={`/profile/${id}/ratings`}>Ratings</Link></button>
            <button className='btnProfile' onClick={handleLogout}> <Link to={`/`}>logout</Link></button>
           </div>
        </div>
        <div className='bookmarkscontainer'>
    <div className='container'>
      <h2>Bookmarks</h2>
      {(!bookmarks || bookmarks.length === 0) ? (<p>You don't have any Bookmarks.</p>) : (
        bookmarks.map(item => (
        <div key={item.titleId + item.viewedAt} className='posterContainer'>
          <img src={item.posterUrl} alt={item.title} className="historyPoster" />
          <div >
            <Link to={`/title/${item.titleId}`}><h3>{item.title}</h3></Link>
            <button onClick={() => {removeBookmark(item.titleId);}}>Remove</button>
          </div>
        </div>
     ))
      )}
    </div>
        </div>
      <Outlet />
    </div>
  );
};

export default Profile;