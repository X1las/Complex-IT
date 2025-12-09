import { useState, useEffect } from 'react';
import {  Link } from 'react-router-dom';
import '../App.css';
import '../css/history.css';
import '../css/bookmarks.css';
import {NL_API} from './search';
import { useAuth } from '../context/AuthContext.jsx';
import DisplayTitleItem from '../services/titlefunctions.jsx';

export  const useAddTitleToHistory = () => {
  const {user} = useAuth();

  const addToHistory = async (titleId) => {
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
      const url = `${NL_API}/api/users/${user.username}/history/${titleId}`;

      const response = await fetch(url, { 
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({username:user.username, titleId:titleId}), credentials: 'include'
      });

      console.log('Add to history response status:', response.status);

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

  return { addToHistory };
};

function useUser() {
  const {user} = useAuth();
  const username = user ? user.username : null;
  return username;
}

function useHistory() {
  const username = useUser();
  const [history, setHistory] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    async function fetchHistory() {
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
        
        const res = await fetch(`${NL_API}/api/users/${username}/history`, { 
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
        setHistory(items);

      } catch (err) {
        console.error('Error fetching history:', err);
        setError('Error fetching history');
      } finally {
        setLoading(false);
      }
    }
    fetchHistory();
  }, [username]);
  
  return { history, error, loading };
}


/* 
function removeHistoryItem(itemId) {
  const clearHistory = () => {};
} */

const History = () => {

  const { history, error, loading } = useHistory();

  const user = useUser();

  if (!user) return <div>Please log in to see your history.</div>;
  if (loading) return <div style={{ padding: 20 }}>Loading history...</div>;
  if (error) return <div className="pagestuff" style={{ top: '500px' }}>Error: {error} Try to sign out and sign in again.</div>;

  return (
    <div className='history-container' style={{ padding: '20px' }}>
      <h2 style={{ color: 'white', marginBottom: '20px' }}>Your Viewing History</h2>
      {(!history || history.length === 0) ? (
        <p style={{ color: 'white' }}>No history available.</p>
      ) : (
        history.map(item => (
          <DisplayTitleItem suppressRating={true} key={item.titleId + item.viewedAt} tconst={item.titleId} />
        ))
      )}
    </div>
  );
};

export default History;
