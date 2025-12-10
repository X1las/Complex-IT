
import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAddTitleToHistory } from '../routes/history';
import { useAuth } from '../context/AuthContext';
import '../css/titlefunctions.css';

const NL_API = 'https://www.newtlike.com:3000';

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
