import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext.jsx';
import {NL_API} from './search';

function useUser() {
  const {user} = useAuth();
  const username = user ? user.username : null;
  return username;
}

export function useRemoveBookmark() {
   const username = useUser();
     const removeBookmark = async (titleId) => {

  if (!username) {
      console.error('No user logged in');
      return;
    }

       try {
        const token = localStorage.getItem('authToken');

      if (!token) {
        console.error('No auth token found');
        return;
      }
     
      const url = `${NL_API}/api/users/${username}/bookmarks/${titleId}`;

      const response = await fetch(url, { 
        method: 'DELETE', 
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
           body: JSON.stringify({ Username: username, TitleId: titleId })
      }); 
      

      if (!response.ok) {
        const txt =  response.text().catch(() => '');
        console.error('Failed to remove bookmark:', response.status, txt);
        return false;
      }

      return true;
    } catch (error) {
      console.error('Error removing bookmark:', error);
      return false;
    }
  
  };
return { removeBookmark };
  
}


export  const useAddBookmarks = () => {
  const {user} = useAuth();



  const addToBookmarks = async (titleId) => {
    if (!user) {
      console.error('No user logged in');
      return;
    }

    try {
      const token = localStorage.getItem('authToken');
      
      if (!token) {
        console.error('No auth token found');
        return;
      }
      const url = `${NL_API}/api/users/${user.username}/bookmarks`;

       const response = await fetch(url, { 
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({username:user.username, titleId:titleId}), credentials: 'include'
      }); 

      if (!response.ok) {
        const txt = await response.text().catch(() => '');
        console.error('Failed to add to history:', response.status, txt.slice(0, 300));
        return;
      } 
         console.log(`Added ${titleId} to history for ${user.username}`);
      return true;
    } catch (error) {
      console.error('Error adding to history:', error);
      return false;
    }
  };

  return { addToBookmarks };
};

export function useBookmarks() {

  const username = useUser();
  const [bookmarks, setBookmarks] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
  async function fetchBookmarks() {
    if (!username) return;
    
    setLoading(true);
    setError(null);

    try {
      const token = localStorage.getItem('authToken');
      
      if (!token) {
        console.error('No auth token found');
        setError('Not authenticated - please login again');
        return;
      }
      
      const res = await fetch(`${NL_API}/api/users/${username}/bookmarks`, { 
        
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        credentials: 'include'
      });

      const historyData = await res.json();

      const items = Array.isArray(historyData.items) ? historyData.items : [];

      console.log('Processed history items:', items.map(i => ({ titleId: i.titleId, viewedAt: i.viewedAt })));
      const titlelist = items.map(item => item.titleId);

      const titleDataArray = await Promise.all(titlelist.map(async (titleId) => {
        const url = `${NL_API}/api/titles/${titleId}`;
        const resp = await fetch(url);
        if (!resp.ok) {
          console.error(`Failed to fetch title data for ${titleId}:`, resp.status);
          return null;
        }
        const titleData = await resp.json();
        return titleData;
      }));

      // Merge history items with title data, keeping viewedAt and other fields
      const data = items.map((item, idx) => {
        const titleData = titleDataArray[idx];
        return {
          ...item,
          title: titleData?.title ?? titleData?.Title ?? item.titleName ?? item.titleId,
          posterUrl: titleData?.posterUrl ?? titleData?.PosterUrl ?? '',
          viewedAt: item.viewedAt 
        };
      });

      console.log('Fetched title data for history items:', data);
      setBookmarks(data);
      

    } catch (err) {
      console.error('Error fetching history:', err);
      setError('Error fetching history');
    } finally {
      setLoading(false);
    }
  }
  fetchBookmarks();
}, [username]);
  return { bookmarks, error, loading };
  
}


const Bookmarks = () => {

}

export default Bookmarks;
