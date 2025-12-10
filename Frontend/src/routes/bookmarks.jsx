/* import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext.jsx';
import {NL_API} from './search';
import { fetchBookmarks, addBookmark, removeBookmark, checkIfBookmarked } from '../services/bookmarksService';
import { Link } from 'react-router';


function useUser() {
  const {user} = useAuth();
  const username = user ? user.username : null;
  return username;
}

// Hook for managing bookmark state (check, add, remove, toggle)
export const useBookmarkState = (titleId) => {
  const { user } = useAuth();
  const [isBookmarked, setIsBookmarked] = useState(false);
  const [loading, setLoading] = useState(false);

  // Check if bookmarked on mount
  useEffect(() => {
    const checkBookmark = async () => {
      if (!user || !titleId) return;
      
      const bookmarked = await checkIfBookmarked(user.username, titleId);
      setIsBookmarked(bookmarked);
    };
    
    checkBookmark();
  }, [user, titleId]);

  // Toggle bookmark (add or remove)
  const toggleBookmark = async () => {
    if (!user) {
      alert('Please login to bookmark');
      return false;
    }

    setLoading(true);
    try {
      if (isBookmarked) {
        await removeBookmark(user.username, titleId);
        setIsBookmarked(false);
      } else {
        await addBookmark(user.username, titleId);
        setIsBookmarked(true);
      }
      return true;
    } catch (error) {
      alert('Failed to update bookmark');
      return false;
    } finally {
      setLoading(false);
    }
  };

  return { isBookmarked, loading, toggleBookmark };
};

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
  const { logout } = useAuth();
  const handleLogout = () => {logout();};

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
      if (!res.ok) {
        console.error('User not signed ind', res.status);
        handleLogout();
        return;
      }

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
 */
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import { fetchBookmarks, addBookmark, removeBookmark, checkIfBookmarked } from '../services/bookmarksService';
import '../css/profile.css';

// Custom hook to get the current username
function useUser() {
  const { user } = useAuth();
  const username = user ? user.username : null;
  return username;
}

// Hook for removing bookmarks
export function useRemoveBookmark() {
  const username = useUser();
  
  const removeBookmarkHandler = async (titleId) => {
    try {
      await removeBookmark(username, titleId);
      return true;
    } catch (error) {
      return false;
    }
  };
  
  return { removeBookmark: removeBookmarkHandler };
}

// Hook for adding bookmarks
export const useAddBookmarks = () => {
  const { user } = useAuth();

  const addToBookmarks = async (titleId) => {
    if (!user) {
      console.error('No user logged in');
      return false;
    }

    try {
      await addBookmark(user.username, titleId);
      return true;
    } catch (error) {
      return false;
    }
  };

  return { addToBookmarks };
};

// Hook for managing bookmark state (check, add, remove, toggle)
export const useBookmarkState = (titleId) => {
  const { user } = useAuth();
  const [isBookmarked, setIsBookmarked] = useState(false);
  const [loading, setLoading] = useState(false);

  // Check if bookmarked on mount
  useEffect(() => {
    const checkBookmark = async () => {
      if (!user || !titleId) return;
      
      const bookmarked = await checkIfBookmarked(user.username, titleId);
      setIsBookmarked(bookmarked);
    };
    
    checkBookmark();
  }, [user, titleId]);

  // Toggle bookmark (add or remove)
  const toggleBookmark = async () => {
    if (!user) {
      alert('Please login to bookmark');
      return false;
    }

    setLoading(true);
    try {
      if (isBookmarked) {
        await removeBookmark(user.username, titleId);
        setIsBookmarked(false);
      } else {
        await addBookmark(user.username, titleId);
        setIsBookmarked(true);
      }
      return true;
    } catch (error) {
      alert('Failed to update bookmark');
      return false;
    } finally {
      setLoading(false);
    }
  };

  return { isBookmarked, loading, toggleBookmark };
};

// Hook for fetching bookmarks
export function useBookmarks() {
  const username = useUser();
  const [bookmarks, setBookmarks] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);


  const loadBookmarks = async () => {
    if (!username) {
      console.log('No username available, skipping bookmark fetch');
      return;
    }
    
    console.log('Loading bookmarks for user:', username);
    setLoading(true);
    setError(null);

    try {
      const data = await fetchBookmarks(username);
      console.log('Successfully fetched bookmarks:', data);
      setBookmarks(data); 
    } catch (err) {
      console.error('Error fetching bookmarks:', err);
      setError(err.message || 'Error fetching bookmarks');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBookmarks();
  }, [username]);
  
  // Function to remove bookmark from local state
  const removeBookmarkFromState = (titleId) => {
    setBookmarks(prev => prev.filter(bookmark => bookmark.titleId !== titleId));
  };
  
  return { bookmarks, error, loading, refetch: loadBookmarks, removeBookmarkFromState };
}

// Bookmarks page component for profile route
const Bookmarks = () => {
  const { error, loading } = useBookmarks();
  const { user } = useAuth();

  if (!user) {
    return (
      <div className="bookmarks-page">
        <h2>Bookmarks</h2>
        <p>Please log in to view your bookmarks.</p>
      </div>
    );
  }

  if (loading) return <div className="bookmarks-page"><p>Loading bookmarks...</p></div>;
  if (error) return <div className="bookmarks-page"><p>Error: {error}</p></div>;

  return (
    <div className="bookmarks-page">
      
    </div>
  );
}

export default Bookmarks;