import '../css/profile.css';
import { useEffect } from 'react';
import { Link, Outlet, useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import {useBookmarks} from './bookmarks.jsx';
import {useRemoveBookmark} from './bookmarks.jsx';
import '../css/bookmarks.css'
/* 
testprofile
ruccer123
 */
function useUser() {
  const {user} = useAuth();
  const username = user ? user.username : null;
  return username;
}

function useTjekifvalidlogin() {
  const {user} = useAuth();
  const token = localStorage.getItem('authToken');
  const { logout } = useAuth();
  const handleLogout = () => {logout();};
  const nav = useNavigate();

  const res = fetch(`https://www.newtlike.com:3000/api/users/${user}/bookmarks`, { 
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });
            //logging out if 401 or 403 == unauthorized
        if (res.status == 401 || res.status === 403) {
        handleLogout();
        console.log('Unauthorized access - logging out');
        nav('/');
        return <div style={{padding: 20}}>User signed out</div>
}
}

const Profile = () => {
    const { id } = useParams();
    const user = useUser();   
    const { bookmarks } = useBookmarks();
    const { removeBookmark } = useRemoveBookmark();

    useTjekifvalidlogin();
  if (!localStorage.getItem('authToken')) return <Link to={`/`}>logout<div style={{padding: 20}}>User not found</div></Link>;
    return (  
      
    <div className='profile'>
      <div className='top'>
        <h1>{user}'s Page</h1>
      </div>

      <div>
          <div className='buttonContainer'>
            <Link to={`/profile/${id}/history`}><button className='btnProfile'>History</button></Link>
            <Link to={`/profile/${id}/ratings`}><button className='btnProfile'>Ratings</button></Link>
            {/* <button className='btnProfile' onClick={handleLogout}> <Link to={`/`}>logout</Link></button>
            */}</div>
        </div>
        <div className='contentContainer'>
        <div className='bookmarksContainer'>
    <div className=''>
      <h2>Bookmarks</h2>
      {(!bookmarks || bookmarks.length === 0) ? (<p>You don't have any Bookmarks.</p>) : (
        bookmarks.map(item => (
        <div key={item.titleId + item.viewedAt} className='bookmarkPosterContainer'>
          <img src={item.posterUrl} alt={item.title} className="bookmarkPoster" />
          
          <div> 
            <Link to={`/title/${item.titleId}`}><h3 className='bookmarkTitle'>{item.title}</h3></Link>
            {console.log('Rendering bookmark item:', item)}
            <div  className='ratingscore'><p  className='rating-value'>{item.rating || 'N/A'}</p> <p className='plot'>{item.plot}</p>  
            <button className='removeBtn' onClick={() => {removeBookmark(item.titleId);}}>Remove</button></div> 
          </div>
        </div>
     ))
      ) }
    </div>
        </div>
        
      <div className="outlet"><Outlet/></div>
    </div>
    </div>
  );
};

export default Profile;