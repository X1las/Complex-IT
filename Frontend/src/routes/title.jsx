import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { fetchUserRating } from '../services/ratingService';
import { StarRatingWidget } from './ratings';
import '../css/title.css';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import logo from '../assets/image.png';
import icon from '../assets/icon.png';
import { useAddBookmarks } from './bookmarks';

const Title = () => {
  const { id: titleId } = useParams();
  const { user } = useAuth();
  const [titleData, setTitleData] = useState(null);
  const [tmdbData, setTmdbData] = useState(null);
  const [castData, setCastData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [userRating, setUserRating] = useState(0);
  const [isBookmarked, setIsBookmarked] = useState(false);
  const [search, setSearch] = useState('');
  const navigate = useNavigate();
  const { addToBookmarks } = useAddBookmarks();

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

  const BookmarkHandler = async (titleId) => {
  return addToBookmarks(titleId)
}

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



  /* console.log('Current user in App component:', user); */
  const onSearchSubmit = (e) => {
    e.preventDefault();
    if (!search.trim()) return;
    navigate(`/search/${(search)}`);
  }

  return (
    <div className="title-page">
      {/* Top Navigation Bar */}
      <nav>
            <div ><Link to="/"><img className='imgPlaceholder' src={logo} alt="Logo" /></Link></div>
            <form onSubmit={onSearchSubmit} style={{display:'inline'}}> 
              <input className='searchField' type="text" value={search} onChange={e => setSearch(e.target.value)} />
            </form>
            <div><Link to={user ? `/profile/${user.username}` : `/login`}> <img className='profileplaceholder' src={icon} alt="profilePic" /></Link></div>
        </nav>

      {/* Header Row */}
      <header className="title-header">
        <h1 className="title-name">{titleData.title}</h1>
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
      </header>

      {/* Main Content - Two Column Layout */}
      <div className="main-content">
        {/* Left Column - Poster */}
        <div className="left-column">
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

        {/* Right Column - Details */}
        <div className="right-column">
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

          {/* Cast Section */}
          <div className="cast-section">
            <h3>Cast</h3>
            <div className="cast-list">
              {castData.length > 0 ? (
                castData.map((member, index) => (
                  <div key={index} className="cast-member">
                    <span className="cast-name">{member.fullname || member.name}</span>
                    {member.character && <span className="cast-character"> as {member.character}</span>}
                  </div>
                ))
              ) : (
                <p>No cast information available.</p>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Right Side Info Card */}
      <aside className="info-card">
        <button 
          className={`bookmark-button`}
          onClick={() => { BookmarkHandler(titleId); console.log(`Bookmark button clicked ${titleId}`); }}
        >
          {isBookmarked ? '★' : '☆'} Bookmark
        </button>

        <div className="metadata-list">
          <div className="metadata-item">
            <span className="metadata-label">Release Date:</span>
            <span className="metadata-value">{titleData.releaseDate || 'N/A'}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Type:</span>
            <span className="metadata-value">{titleData.titleType}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Mature Content:</span>
            <span className="metadata-value">{titleData.isAdult}</span>
          </div>
          <div className="metadata-item">
            <span className="metadata-label">Website:</span>
            <span className="metadata-value">
              {titleData.websiteUrl ? <a href={titleData.websiteUrl} target="_blank" rel="noopener noreferrer">Visit</a> : 'N/A'}
            </span>
          </div>
          {titleData.endYear && (
            <div className="metadata-item">
              <span className="metadata-label">End Year:</span>
              <span className="metadata-value">{titleData.endYear}</span>
            </div>
          )}
        </div>
      </aside>
    </div>
  );
};

export default Title;