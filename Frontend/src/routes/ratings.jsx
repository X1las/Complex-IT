import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext.jsx';
import { submitRating, deleteRating, fetchAllUserRatings } from '../services/ratingService';

// Custom hook to get the current username
function useUser() {
  const { user } = useAuth();
  const username = user ? user.username : null;
  return username;
}

// Hook for fetching all user ratings
export function useUserRatings() {
  const username = useUser();
  const [ratings, setRatings] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const loadRatings = async () => {
    if (!username) return;
    
    setLoading(true);
    setError(null);

    try {
      const data = await fetchAllUserRatings(username);
      setRatings(data); 
    } catch (err) {
      console.error('Error fetching ratings:', err);
      setError(err.message || 'Error fetching ratings');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRatings();
  }, [username]);
  
  // Function to remove rating from local state
  const removeRatingFromState = (titleId) => {
    setRatings(prev => prev.filter(rating => rating.titleId !== titleId));
  };
  
  return { ratings, error, loading, refetch: loadRatings, removeRatingFromState };
}

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

// Ratings page component - displays all user ratings
const Ratings = () => {
  const { ratings, error, loading, removeRatingFromState } = useUserRatings();
  const { user } = useAuth();

  const handleDeleteRating = async (titleId) => {
    if (!confirm('Delete your rating?')) return;
    
    const success = await deleteRating(user, titleId);
    if (success) {
      removeRatingFromState(titleId);
    }
  };

  if (loading) return <div className="ratings-container"><p>Loading ratings...</p></div>;
  if (error) return <div className="ratings-container"><p>Error: {error}</p></div>;

  return (
    <div className="ratings-container">
      <h2>Your Ratings</h2>
      {(!ratings || ratings.length === 0) ? (
        <p>You haven't rated any titles yet.</p>
      ) : (
        <div className="ratings-list">
          {ratings.map(item => (
            <div key={item.titleId} className="rating-item">
              {item.posterUrl && <img src={item.posterUrl} alt={item.title} className="rating-poster" />}
              <div className="rating-details">
                <h3>{item.title}</h3>
                <p>Your rating: {item.rating}/10 ⭐</p>
                <button className="delete-rating-btn" onClick={() => handleDeleteRating(item.titleId)}>
                  Delete Rating
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Ratings;