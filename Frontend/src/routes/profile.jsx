import '../css/profile.css';
import { Link, Outlet, useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import {useBookmarks} from '../routes/bookmarks.jsx';
import {useRemoveBookmark} from '../routes/bookmarks.jsx';
import '../css/bookmarks.css'
import {maxTegn} from './search.jsx';
import { useEffect } from 'react';
/* 
testprofile
ruccer123
 */


function useTjekifvalidlogin() {
  //const username = useUser();
  const token = localStorage.getItem('authToken');
  const { logout } = useAuth();
  const nav = useNavigate();
  const username = useUser();
  

  useEffect(  () => {
   if (!username) return;
const checkLogin = async () => {
  const res = await fetch(`https://www.newtlike.com:3000/api/users/${username}/bookmarks`, { 
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    })
  
        //logging out if 401 or 403 == unauthorized
        console.log('KIG HER !!!!', res);
        if (res.status == 401 || res.status == 403) {
        alert('Session expired, please log in again.');
        logout();
        console.log('Unauthorized access - logging out');
        nav('/');
        return <div style={{padding: 20}}>User signed out</div>
    }
  };
  checkLogin();
  },[username]);
}

function useUser () {
    const {user} = useAuth();
  // if (user == null) return null;
   const username = user ? user.username : null;
  return username;
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
            <div  className='ratingscore'><p  className='rating-value'>{item.rating || 'N/A'}</p> <p className='plot'>{maxTegn(item.plot, 100)}</p>  
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