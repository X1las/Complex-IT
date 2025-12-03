import { useState } from 'react';
import { submitRating, deleteRating } from '../utils/ratingService';

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

// Ratings page component
const Ratings = () => {
  return (
    <div className="ratings-container">
      <h1>Ratings</h1>
      <p>Rating management functionality is available on individual title pages.</p>
    </div>
  );
};

export default Ratings;
