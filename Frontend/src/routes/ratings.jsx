import { useState, useEffect } from 'react';
import { submitRating, deleteRating } from '../services/ratingService';
import { useAuth } from '../context/AuthContext';
import '../App.css';
import '../css/ratings.css';
import DisplayTitleItem from '../services/titlefunctions.jsx';

const NL_API = 'https://www.newtlike.com:3000';

// Star Rating Widget Component
export const StarRatingWidget = ({ user, titleId, userRating, onRatingChange, onRatingDelete }) => {
  const [hoveredStar, setHoveredStar] = useState(0);

  const handleStarClick = async (rating) => {
    const success = await submitRating(user, titleId, rating);
    if (success && onRatingChange) onRatingChange(rating);
  };

  const handleDelete = async () => {
    const success = await deleteRating(user, titleId);
    if (success && onRatingDelete) onRatingDelete();
  };

  if (!user) return null;

  return (
    <div className="user-rating-widget">
      <div className="star-rating-container">
        {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map(star => (
          <button
            key={star}
            className={`star-button ${star <= (hoveredStar || userRating) ? 'filled' : ''}`}
            onClick={() => handleStarClick(star)}
            onMouseEnter={() => setHoveredStar(star)}
            onMouseLeave={() => setHoveredStar(0)}
          >
            ★
          </button>
        ))}
        <span className="rating-display">{hoveredStar || userRating || 0}/10</span>
      </div>
      {userRating > 0 && (
        <button className="delete-rating-btn" onClick={handleDelete}>Delete</button>
      )}
    </div>
  );
};

const FetchUserRatings = async (username) => {
  try {
    const token = localStorage.getItem('authToken');
    console.log('Fetching ratings for username:', username);
    console.log('Using token:', token ? 'Token found' : 'No token');
    console.log('API URL:', `${NL_API}/api/users/${username}/ratings`);
    
    const res = await fetch(`${NL_API}/api/users/${username}/ratings`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });
    
    console.log('Response status:', res.status);
    console.log('Response ok:', res.ok);
    
    if (!res.ok) {
      console.error('Failed to fetch ratings:', res.status);
      const errorText = await res.text();
      console.error('Error response:', errorText);
      return [];
    }
    const data = await res.json();
    console.log('Received data:', data);
    return data.items || [];
  } catch (err) {
    console.error('Error fetching ratings:', err);
    return [];
  }
};

// Ratings page component
const Ratings = () => {
  const { user } = useAuth();
  const [userRatings, setUserRatings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchRatings = async () => {
      console.log('useEffect triggered, user:', user);
      if (user && user.username) {
        try {
          setLoading(true);
          console.log('Starting to fetch ratings...');
          const ratings = await FetchUserRatings(user.username);
          console.log('Fetched ratings:', ratings);
          setUserRatings(ratings);
        } catch (err) {
          setError('Failed to load ratings');
          console.error('Error loading ratings:', err);
        } finally {
          setLoading(false);
        }
      } else {
        console.log('No user or username available');
        setLoading(false);
      }
    };
    fetchRatings();
  }, [user]);

  if (loading) return <div className="ratings-container"><p>Loading ratings...</p></div>;
  if (error) return <div className="ratings-container"><p>Error: {error}</p></div>;
  if (!user) return <div className="ratings-container"><p>Please log in to view your ratings.</p></div>;

  return (
    <div className="ratings-container">
      <h2>Your Ratings</h2>
      <p>Rating management functionality is available on individual title pages.</p>
      <div className="user-ratings" style={{
        display: 'flex',
        flexDirection: 'column',
        marginTop: '20px'
      }}>
        {userRatings.length > 0 ? (
          userRatings.map((rating, index) => (
            <DisplayTitleItem suppressDate={true} key={rating.id || index} tconst={rating.titleId} />
          ))
        ) : (
          <p>You haven't rated any titles yet.</p>
        )}
      </div>
    </div>
  );
};

export default Ratings;