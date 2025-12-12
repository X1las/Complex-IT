import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { fetchUserRating } from '../services/ratingService';
import { StarRatingWidget } from './ratings';
import '../css/title.css';
import '../App.css';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import logo from '../assets/image.png';
import icon from '../assets/icon.png';
import { useBookmarkState } from './bookmarks';

const Title = () => {
  const { id: titleId } = useParams();
  const { user } = useAuth();
  const [titleData, setTitleData] = useState(null);
  const [tmdbData, setTmdbData] = useState(null);
  const [castData, setCastData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [userRating, setUserRating] = useState(0);
  const [search, setSearch] = useState('');
  const navigate = useNavigate();
  const { isBookmarked, loading: bookmarkLoading, toggleBookmark } = useBookmarkState(titleId);

  const fetchTitleDetails = async (titleId) => {
    
    try {
      setLoading(true);
      console.log('Fetching title:', titleId);
      
      // Fetch internal data from database
      const response = await fetch(`https://newtlike.com:3000/api/titles/${titleId}`);
      console.log('Database response status:', response.status);
      if (!response.ok) {
        throw new Error(`Failed to fetch title details: ${response.status}`);
      }
      const data = await response.json();
      console.log('Title data from database:', data);
      setTitleData(data);

      // Check what data is missing from database
      const needsPoster = !data.posterUrl;
      const needsPlot = !data.plot || data.plot.trim() === '';
      const needsAdditionalInfo = needsPoster || needsPlot;

      // Only fetch from TMDB if we're missing data
      if (needsAdditionalInfo) {
        console.log('Missing data from database. Fetching from TMDB...');
        console.log('Needs poster:', needsPoster, '| Needs plot:', needsPlot);
        
        const externalRes = await fetch(`https://api.themoviedb.org/3/find/${titleId}?external_source=imdb_id&api_key=f7cb406cd3ce48761cb8749ec2be8e89`);
        console.log('TMDB response status:', externalRes.status);
        
        if (externalRes.ok) {
          const externalData = await externalRes.json();
          console.log('External data from TMDB:', externalData);
          const tmdbResult = externalData.movie_results?.[0] || externalData.tv_results?.[0] || null;
          
          if (tmdbResult) {
            console.log('TMDB data found:', {
              hasPoster: !!tmdbResult.poster_path,
              hasOverview: !!tmdbResult.overview
            });
            setTmdbData(tmdbResult);
          } else {
            console.log('No TMDB data found for this ID');
          }
        } else {
          console.log('TMDB API request failed with status:', externalRes.status);
        }
      } else {
        console.log('All data available from database. Skipping TMDB fetch.');
      }

      // Fetch cast/crew data
      console.log('Fetching cast data...');
      const crewRes = await fetch(`https://newtlike.com:3000/api/titles/${titleId}/crew`);
      if (crewRes.ok) {
        const crewData = await crewRes.json();
        console.log('Cast data received:', crewData);
        
        // Fetch detailed info for each crew member
        if (crewData.items && crewData.items.length > 0) {
          const detailedCast = await Promise.all(
            crewData.items.slice(0, 10).map(async (member) => {
              try {
                const detailRes = await fetch(`https://newtlike.com:3000${member.url}`);
                if (detailRes.ok) {
                  return await detailRes.json();
                }
                return null;
              } catch (err) {
                console.error('Error fetching crew member:', err);
                return null;
              }
            })
          );
          const validCast = detailedCast.filter(c => c !== null);
          console.log('Detailed cast data:', validCast);         
          setCastData(validCast);
        }
      } else {
        console.log('Cast API failed with status:', crewRes.status);
      }
    } catch (error) {
      console.error('Error fetching title details:', error);
      setTitleData(null);
    } finally {
      setLoading(false);
    }
  };

  // const fetchSimilarTitles = () => {
  //   // TODO: Fetch similar/related titles
  // };

  // const shareTitle = () => {
  //   // TODO: Share title functionality
  // };

  useEffect(() => {
    if (titleId) {
      fetchTitleDetails(titleId);
      if (user) {
        fetchUserRating(user, titleId).then(rating => setUserRating(rating));
      }
    }
  }, [titleId, user]);

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (!titleData) {
    return <div className="error">Title not found</div>;
  }
  
console.log('DET ER HER ', castData);
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
            fetchTitleDetails(titleId); // Refresh title data to get updated rating
          }}
          onRatingDelete={() => {
            setUserRating(0);
            fetchTitleDetails(titleId); // Refresh title data to get updated rating
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
                  <img src={icon} alt={member.fullname || member.name} />
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