import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';

const Title = () => {
  const { titleId } = useParams();
  const [titleData, setTitleData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [userRating, setUserRating] = useState(0);
  const [isBookmarked, setIsBookmarked] = useState(false);

  const fetchTitleDetails = async (titleId) => {
    try {
      setLoading(true);
      // TODO: Replace with your actual API endpoint
      const response = await fetch(`/api/titles/${titleId}`);
      if (!response.ok) {
        throw new Error('Failed to fetch title details');
      }
      const data = await response.json();
      setTitleData(data);
    } catch (error) {
      console.error('Error fetching title details:', error);
      setTitleData(null);
    } finally {
      setLoading(false);
    }
  };

  const addToBookmarks = () => {
    // TODO: Add title to user bookmarks
    setIsBookmarked(!isBookmarked);
  };

  const submitRating = (rating) => {
    // TODO: Submit user rating for this title
    setUserRating(rating);
  };

  const fetchSimilarTitles = () => {
    // TODO: Fetch similar/related titles
  };

  const shareTitle = () => {
    // TODO: Share title functionality
  };

  useEffect(() => {
    // TODO: Replace with real API call
    if (titleId) {
      fetchTitleDetails(titleId);
    }
  }, [titleId]);

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (!titleData) {
    return <div className="error">Title not found</div>;
  }

  return (
    <div className="title-page">
      {/* Top Navigation Bar */}
      <nav className="top-nav">
        <div className="logo">MovieDB</div>
        <div className="search-container">
          <input type="text" placeholder="Search movies..." className="search-bar" />
          <button className="search-icon"></button>
        </div>
        <button className="user-icon"></button>
      </nav>

      {/* Header Row */}
      <header className="title-header">
        <h1 className="title-name">{titleData.title}</h1>
        <div className="rating-section">
          <span className="rating"> {titleData.rating}</span>
          <span className="rating-count">({titleData.ratingCount.toLocaleString()} ratings)</span>
        </div>
        <button className="rate-button" onClick={() => submitRating(userRating)}>
          Rate Movie
        </button>
      </header>

      {/* Main Content - Two Column Layout */}
      <div className="main-content">
        {/* Left Column - Poster */}
        <div className="left-column">
          <img src={titleData.poster} alt={titleData.title} className="poster-image" />
        </div>

        {/* Right Column - Details */}
        <div className="right-column">
          {/* Runtime and Year Box */}
          <div className="meta-box">
            <span className="runtime">{titleData.runtime} min</span>
            <span className="year">{titleData.year}</span>
          </div>

          {/* Description */}
          <div className="description">
            <h3>Overview</h3>
            <p>{titleData.description}</p>
          </div>

          {/* Cast Section */}
          <div className="cast-section">
            <h3>Cast</h3>
            <div className="cast-list">
              {titleData.cast.map((member) => (
                <div key={member.id} className="cast-item">
                  <img src={member.portrait} alt={member.name} className="actor-portrait" />
                  <div className="cast-info">
                    <div className="actor-name">{member.name}</div>
                    <div className="role-name">{member.role}</div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Right Side Info Card */}
      <aside className="info-card">
        <button 
          className={`bookmark-button ${isBookmarked ? 'bookmarked' : ''}`}
          onClick={addToBookmarks}
        >
          {isBookmarked ? '★' : '☆'} Bookmark
        </button>

        <div className="metadata-list">
          <div className="metadata-item">
            <span className="metadata-label">Release Date:</span>
            <span className="metadata-value">{titleData.releaseDate}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Age Rating:</span>
            <span className="metadata-value">{titleData.ageRating}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Mature Content:</span>
            <span className="metadata-value">{titleData.matureContent}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Box Office:</span>
            <span className="metadata-value">{titleData.boxOffice}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Director:</span>
            <span className="metadata-value">{titleData.director}</span>
          </div>
          {titleData.seasons && (
            <div className="metadata-item">
              <span className="metadata-label">Seasons:</span>
              <span className="metadata-value">{titleData.seasons}</span>
            </div>
          )}
          {titleData.episodes && (
            <div className="metadata-item">
              <span className="metadata-label">Episodes:</span>
              <span className="metadata-value">{titleData.episodes}</span>
            </div>
          )}
        </div>
      </aside>
    </div>
  );
};

export default Title;
