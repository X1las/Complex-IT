import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAddTitleToHistory } from '../routes/history';
import { useAuth } from '../context/AuthContext';
import '../css/titleFunctions.css';

const NL_API = 'https://www.newtlike.com:3000';
const TMDB_API_KEY = 'f7cb406cd3ce48761cb8749ec2be8e89';

// Custom hook for fetching complete title details with TMDB data and cast
export const useTitleDetails = (titleId) => {
    const [titleData, setTitleData] = useState(null);
    const [tmdbData, setTmdbData] = useState(null);
    const [castData, setCastData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!titleId) {
            setLoading(false);
            return;
        }

        const fetchTitleDetails = async () => {
            try {
                setLoading(true);
                setError(null);
                console.log('Fetching title:', titleId);

                // Fetch internal data from database
                const response = await fetch(`${NL_API}/api/titles/${titleId}`);
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

                    const externalRes = await fetch(
                        `https://api.themoviedb.org/3/find/${titleId}?external_source=imdb_id&api_key=${TMDB_API_KEY}`
                    );
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
                const crewRes = await fetch(`${NL_API}/api/titles/${titleId}/crew`);
                
                if (crewRes.ok) {
                    const crewData = await crewRes.json();
                    console.log('Cast data received:', crewData);

                    // Fetch detailed info for each crew member including TMDB profile image
                    if (crewData.items && crewData.items.length > 0) {
                        const detailedCast = await Promise.all(
                            crewData.items.slice(0, 10).map(async (member) => {
                                try {
                                    const detailRes = await fetch(`${NL_API}${member.url}`);
                                    if (detailRes.ok) {
                                        const crewDetails = await detailRes.json();

                                        // Fetch TMDB image for this crew member
                                        try {
                                            const tmdbRes = await fetch(
                                                `https://api.themoviedb.org/3/find/${crewDetails.crewId}?external_source=imdb_id&api_key=${TMDB_API_KEY}`
                                            );
                                            if (tmdbRes.ok) {
                                                const tmdbData = await tmdbRes.json();
                                                const personData = tmdbData.person_results?.[0];
                                                if (personData?.profile_path) {
                                                    crewDetails.profilePath = personData.profile_path;
                                                }
                                            }
                                        } catch (tmdbErr) {
                                            console.log('TMDB fetch failed for crew member:', crewDetails.crewId);
                                        }

                                        return crewDetails;
                                    }
                                    return null;
                                } catch (err) {
                                    console.error('Error fetching crew member:', err);
                                    return null;
                                }
                            })
                        );
                        const validCast = detailedCast.filter(c => c !== null);
                        console.log('Detailed cast data with images:', validCast);
                        setCastData(validCast);
                    }
                } else {
                    console.log('Cast API failed with status:', crewRes.status);
                }
            } catch (err) {
                console.error('Error fetching title details:', err);
                setError(err.message);
                setTitleData(null);
            } finally {
                setLoading(false);
            }
        };

        fetchTitleDetails();
    }, [titleId]);

    return {
        titleData,
        tmdbData,
        castData,
        loading,
        error
    };
};

const DisplayTitleItem = ({ tconst, suppressDate = false, suppressRating = false }) => {
    const [title, setTitle] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [userRating, setUserRating] = useState(null);
    const [visitedAt, setVisitedAt] = useState(null);
    const { HistoryHandler } = useAddTitleToHistory();
    const { user } = useAuth();

    useEffect(() => {
        const fetchTitle = async () => {
            try {
                setLoading(true);
                
                const response = await fetch(`${NL_API}/api/titles/${tconst}`);
                if (!response.ok) {
                    throw new Error(`Failed to fetch title: ${response.status}`);
                }
                const titleData = await response.json();
                setTitle(titleData);

                // Fetch user-specific data only if user is logged in
                if (user?.username) {
                    // Fetch history data unless suppressed
                    if (!suppressDate) {
                        try {
                            const historyResp = await fetch(`${NL_API}/api/users/${user.username}/history`, {
                                method: 'GET',
                                headers: {
                                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
                                    'Content-Type': 'application/json'
                                }
                            });
                            if (historyResp.ok) {
                                const historyData = await historyResp.json();
                                const historyItems = historyData.items || [];
                                const viewedItem = historyItems.find(item => item.titleId === tconst);
                                setVisitedAt(viewedItem?.viewedAt || null);
                            }
                        } catch (historyErr) {
                            console.error('Error fetching history:', historyErr);
                        }
                    }

                    // Fetch rating data unless suppressed
                    if (!suppressRating) {
                        try {
                            const ratingsResp = await fetch(`${NL_API}/api/users/${user.username}/ratings`, {
                                method: 'GET',
                                headers: {
                                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
                                    'Content-Type': 'application/json'
                                }
                            });
                            if (ratingsResp.ok) {
                                const ratingsData = await ratingsResp.json();
                                const ratingItems = ratingsData.items || [];
                                const userTitleRating = ratingItems.find(item => item.titleId === tconst);
                                setUserRating(userTitleRating?.rating || null);
                            }
                        } catch (ratingErr) {
                            console.error('Error fetching rating:', ratingErr);
                        }
                    }
                }
            } catch (err) {
                setError(err.message);
                console.error('Error fetching title:', err);
            } finally {
                setLoading(false);
            }
        };

        if (tconst) {
            fetchTitle();
        } else {
            setLoading(false);
        }
    }, [tconst, user?.username, suppressDate, suppressRating]);

    if (loading) return <div className="title-loading">Loading...</div>;
    if (error) return <div className="title-error">Error: {error}</div>;
    if (!title) return null;

    return (
        <div className='title-item-container'>
            <div className='title-item-card' key={title.id || tconst}>
                <div className='title-poster'>
                    <Link to={`/title/${title.id || tconst}`} onClick={() => HistoryHandler(title.id || tconst)}>
                        <img src={title.poster_url || title.posterUrl} alt={title.title || title.primaryTitle} />
                    </Link>
                </div>
                <Link to={`/title/${title.id || tconst}`} className='title-link' onClick={() => HistoryHandler(title.id || tconst)}>
                    <p className='title-names'>{title.title || title.primaryTitle}</p>
                    <div className='title-details'>
                        <p>Rating: {title.rating || 'N/A'}</p>
                        <p>Year: {title.year || title.startYear || 'N/A'}</p>
                        <p>Type: {title.titleType || 'N/A'}</p>
                        {userRating && <p>Your Rating: {userRating}/10</p>}
                        {visitedAt && <p>Visited: {new Date(visitedAt).toLocaleDateString()}</p>}
                    </div>
                </Link>
            </div>
        </div>
    );
};

export default DisplayTitleItem;
