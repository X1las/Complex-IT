
import { useState, useEffect } from 'react';
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