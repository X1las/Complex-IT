import '../css/profile.css';
import { Link, Outlet, useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import {useBookmarks} from '../routes/bookmarks.jsx';
import {useRemoveBookmark} from '../routes/bookmarks.jsx';
import '../css/bookmarks.css'
import {maxTegn} from './search.jsx';
import { useEffect } from 'react';
import { Button, ButtonGroup } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';

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
  
    if (res.status == 401 || res.status == 403) {
      logout();
      nav('/');
      return <div style={{padding: 20}}>User signed out</div>
    }
  };
  checkLogin();
  },[username]);
}

function useUser () {
  const {user} = useAuth();
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
            <ButtonGroup className="mb-3" style={{ gap: '10px' }}>
              <Button 
                as={Link} 
                to={`/profile/${id}/history`} 
                className="btnProfile"
                size="lg"
              >
                History
              </Button>
              <Button 
                as={Link} 
                to={`/profile/${id}/ratings`} 
                className="btnProfile"
                size="lg"
              >
                Ratings
              </Button>
            </ButtonGroup>
          </div>
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
            <div  className='ratingscore'>
              <p  className='rating-value'>{item.rating || 'N/A'}</p> 
              <p className='plot'>{maxTegn(item.plot, 100)}</p>  
              <Button 
                variant="danger" 
                size="sm" 
                onClick={() => {removeBookmark(item.titleId);}}
              >
                Remove
              </Button>
            </div> 
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