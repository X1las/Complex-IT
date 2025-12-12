import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { fetchUserRating } from '../services/ratingService';
import { useTitleDetails } from '../services/titleFunctions';
import { StarRatingWidget } from './ratings';
import { useBookmarkState } from './bookmarks';
import icon from '../assets/icon.png';
import '../css/title.css';
import '../App.css';

const Title = () => {
  const { id: titleId } = useParams();
  const { user } = useAuth();
  const [userRating, setUserRating] = useState(0);
  const { isBookmarked, loading: bookmarkLoading, toggleBookmark } = useBookmarkState(titleId);
  
  // Use custom hook for fetching title details
  const { titleData, tmdbData, castData, loading, error } = useTitleDetails(titleId);

  // Fetch user rating when component mounts or user/titleId changes
  useEffect(() => {
    if (user && titleId) {
      fetchUserRating(user, titleId).then(rating => setUserRating(rating));
    }
  }, [titleId, user]);

  // Loading and error states
  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error) {
    return <div className="error">Error: {error}</div>;
  }

  if (!titleData) {
    return <div className="error">Title not found</div>;
  }
  return (
    <div className="title-page">

      {/* Header Row - Title and Rating */}
      <header className="title-header">
        <h1 className="title-name">{titleData.title}</h1>
      </header>

      <div className="rating-row">
        <div className="rating-section">
          <span className="rating">★ {titleData.rating}</span>
          <span className="rating-count">({titleData.votes?.toLocaleString()} votes)</span>
        </div>
        <StarRatingWidget 
          user={user}
          titleId={titleId}
          userRating={userRating}
          onRatingChange={(rating) => {
            setUserRating(rating);
            // Rating updated - page will reflect changes on next reload
          }}
          onRatingDelete={() => {
            setUserRating(0);
            // Rating deleted - page will reflect changes on next reload
          }}
        />
      </div>

      {/* Main Content Grid */}
      <div className="content-grid">
        {/* Left Column - Poster */}
        <div className="poster-column">
          {(titleData.posterUrl || tmdbData?.poster_path) ? (
            <img 
              src={titleData.posterUrl || `https://image.tmdb.org/t/p/w500${tmdbData.poster_path}`} 
              alt={titleData.title} 
              className="poster-image" 
            />
          ) : (
            <div className="poster-placeholder">No Poster Available</div>
          )}
        </div>

        {/* Middle Column - Description and Basic Info */}
        <div className="description-column">
          {/* Runtime and Year Box */}
          <div className="meta-box">
            <span className="runtime">{titleData.runtimeMinutes} min</span>
            <span className="year">{titleData.startYear}</span>
          </div>

          {/* Description */}
          <div className="description">
            <h3>Overview</h3>
            <p>{titleData.plot || tmdbData?.overview || 'No plot available.'}</p>
          </div>
        </div>

        {/* Right Sidebar - Bookmark & Metadata */}
        <aside className="info-sidebar">
          <button 
            className={`bookmark-button ${isBookmarked ? 'bookmarked' : ''}`}
            onClick={toggleBookmark}
            disabled={bookmarkLoading}
          >
            {bookmarkLoading ? 'Loading...' : (isBookmarked ? '★ Bookmarked' : '☆ Bookmark Movie')}
          </button>

          <div className="metadata-list">
            <div className="metadata-item">
              <span className="metadata-label">Release Date</span>
              <span className="metadata-value">{titleData.releaseDate || 'N/A'}</span>
            </div>
            <div className="metadata-item">
              <span className="metadata-label">Age Rating</span>
              <span className="metadata-value">{titleData.isAdult ? 'Mature' : 'General'}</span>
            </div>
            <div className="metadata-item">
              <span className="metadata-label">Mature Content</span>
              <span className="metadata-value">{titleData.isAdult}</span>
            </div>
            <div className="metadata-item">
              <span className="metadata-label">Box Office</span>
              <span className="metadata-value">N/A</span>
            </div>
            <div className="metadata-item">
              <span className="metadata-label">Director</span>
              <span className="metadata-value">N/A</span>
            </div>
            {titleData.endYear && (
              <div className="metadata-item">
                <span className="metadata-label">Season/Episode</span>
                <span className="metadata-value">{titleData.endYear}</span>
              </div>
            )}
          </div>
        </aside>
      </div>

      {/* Cast Section - Full Width Below */}
      <div className="cast-section">
        <h3>Cast</h3>
        <div className="cast-grid">
          {castData.length > 0 ? (
            castData.map((member, index) => (
              <Link to={`/person/${member.crewId}`} key={index} className="cast-member">
                <div className="cast-portrait">
                  <img 
                    src={member.profilePath ? `https://image.tmdb.org/t/p/w200${member.profilePath}` : icon} 
                    alt={member.fullname || member.name}
                    onError={(e) => { e.target.src = icon; }}
                  />
                </div>
                <div className="cast-info">
                  <div className="cast-role">{member.character || 'Role'}</div>
                  <div className="cast-name">{member.fullname || member.name}</div>
                </div>
              </Link>
            ))
          ) : (
            <p>No cast information available.</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default Title;