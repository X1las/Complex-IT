import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAddTitleToHistory } from '../routes/history';
import { useAuth } from '../context/AuthContext';
import '../css/titleFunctions.css';
import { getPerson } from '../routes/person.jsx';

const NL_API = 'https://www.newtlike.com:3000';
const TMDB_API_KEY = 'f7cb406cd3ce48761cb8749ec2be8e89';

// Custom hook for fetching complete title details with TMDB data and cast
export const useTitleDetails = (titleId) => {
    const [titleData, setTitleData] = useState(null);
    const [tmdbData, setTmdbData] = useState(null);
    const [castData, setCastData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [refetchTrigger, setRefetchTrigger] = useState(0);

    useEffect(() => {
        if (!titleId) {
            setLoading(false);
            return;
        }

        const fetchTitleDetails = async () => {
            try {
                setLoading(true);
                setError(null);

                const response = await fetch(`${NL_API}/api/titles/${titleId}`);
                
                if (!response.ok) {
                    throw new Error(`Failed to fetch title details: ${response.status}`);
                }
                
                const data = await response.json();
                setTitleData(data);


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

                    if (externalRes.ok) {
                        const externalData = await externalRes.json();
                        const tmdbResult = externalData.movie_results?.[0] || externalData.tv_results?.[0] || null;

                        if (tmdbResult) {
                            setTmdbData(tmdbResult);
                        }
                    }
                }

                const crewRes = await fetch(`${NL_API}/api/titles/${titleId}/crew`);
                
                if (crewRes.ok) {
                    const crewData = await crewRes.json();

                    if (crewData.items && crewData.items.length > 0) {
                        const detailedCast = await Promise.all(
                            crewData.items.slice(0, 10).map(async (member) => {
                                try {
                                    const detailRes = await fetch(`${NL_API}${member.url}`);
                                    if (detailRes.ok) {
                                        const crewDetails = await detailRes.json();

                                        try {
                                            const personData = await getPerson(crewDetails.crewId);
                                            return personData || crewDetails;
                                        } catch (personErr) {
                                            return crewDetails;
                                        }
                                    }
                                    return null;
                                } catch (err) {
                                    return null;
                                }
                            })
                        );
                        const validCast = detailedCast.filter(c => c !== null);
                        setCastData(validCast);
                    }
                }
            } catch (err) {
                setError(err.message);
                setTitleData(null);
            } finally {
                setLoading(false);
            }
        };

        fetchTitleDetails();
    }, [titleId, refetchTrigger]);

    const refetch = () => {
        setRefetchTrigger(prev => prev + 1);
    };

    return {
        titleData,
        tmdbData,
        castData,
        loading,
        error,
        refetch
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

                if (user?.username) {
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
                        } catch (historyErr) {}
                    }

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
                        } catch (ratingErr) {}
                    }
                }
            } catch (err) {
                setError(err.message);
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