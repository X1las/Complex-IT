import { useState, useEffect } from 'react';
import {  Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import '../App.css';
import '../css/history.css';
import '../css/bookmarks.css';

const NL_API = 'https://www.newtlike.com:3000';

// Custom hook for adding titles to history
export const useAddTitleToHistory = () => {
  const { user } = useAuth();

  const addToHistory = async (titleId) => {
    if (!user?.username) {
      console.error('No user logged in');
      return false;
    }

    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        console.error('No auth token found');
        return false;
      }

      const response = await fetch(`${NL_API}/api/users/${user.username}/history/${titleId}`, { 
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username: user.username, titleId }),
        credentials: 'include'
      });

      if (!response.ok) {
        const errorText = await response.text().catch(() => '');
        console.error('Failed to add to history:', response.status, errorText.slice(0, 300));
        return false;
      }

      console.log(`Added ${titleId} to history for ${user.username}`);
      return true;
    } catch (error) {
      console.error('Error adding to history:', error);
      return false;
    }
  };

  return { addToHistory, HistoryHandler: addToHistory };
};

// Get current user helper
function useUser() {
  const { user } = useAuth();
  return user?.username || null;
}

// Custom hook for fetching user history

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
      setHistory(data);
      

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

// Main History component
const History = () => {
  const { history, error, loading } = useHistory();
  const user = useUser();

  if (!user) return <div>Please log in to see your history.</div>;
  if (loading) return <div style={{ padding: 20 }}>Loading history...</div>;
  if (error) return <div className="pagestuff" style={{ top: '500px' }}>Error: {error} Try to sign out and sign in again.</div>;

  return (
    <div className='history-container'>
      <h2 style={{ color: 'white', marginBottom: '20px' }}>Your Viewing History</h2>
      {(!history || history.length === 0) ? (
        <p style={{ color: 'white' }}>No history available.</p>
      ) : (
        history.map(item => (
        <div key={item.titleId + item.viewedAt} className='posterContainer'>
          <img src={item.posterUrl ? item.posterUrl : null} alt={'Image'} className="historyPoster" />
          <div className="historyTitle">
            <Link to={`/title/${item.titleId}`}><span ><p className='Htitle'>{item.title}</p></span>
            <div className="historyDetails">Viewed at: {new Date(item.viewedAt).toLocaleDateString()}</div></Link>
          </div>
        </div>
     ))
      )}
    </div>
  );
};

export default History;
