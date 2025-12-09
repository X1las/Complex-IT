import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAddTitleToHistory } from '../routes/history';
import { useAuth } from '../context/AuthContext';
import '../css/titlefunctions.css';

const NL_API = 'https://www.newtlike.com:3000';

const DisplayTitleItem = ({ tconst, suppressDate=false, suppressRating=false }) => {
    const [title, setTitle] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [userRating, setUserRating] = useState(null);
    const [visitedAt, setVisitedAt] = useState(null);
    const { HistoryHandler } = useAddTitleToHistory();
    const { user } = useAuth();

    console.log('DisplayTitleItem component rendered with tconst:', tconst);

    useEffect(() => {
        console.log('useEffect triggered with tconst:', tconst);

        const fetchTitle = async () => {
            try {
                setLoading(true);
                console.log('Fetching title for tconst:', tconst);
                console.log('API URL:', `${NL_API}/api/titles/${tconst}`);

                const response = await fetch(`${NL_API}/api/titles/${tconst}`);
                console.log('Response status:', response.status);
                console.log('Response ok:', response.ok);

                if (!response.ok) {
                    throw new Error(`Failed to fetch title: ${response.status}`);
                }
                const titleData = await response.json();
                console.log('Received title data:', titleData);
                setTitle(titleData);

                // Check user-specific data only if user is logged in
                if (user && user.username) {
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
                                console.log('Received history data:', historyData);
                                const historyItems = historyData.items || [];
                                const viewedItem = historyItems.find(item => item.titleId === tconst);
                                console.log('Found viewed item:', viewedItem);
                                setVisitedAt(viewedItem ? viewedItem.viewedAt : null);
                            } else {
                                console.log('No history found or failed to fetch history:', historyResp.status);
                                setVisitedAt(null);
                            }
                        } catch (historyErr) {
                            console.error('Error fetching history:', historyErr);
                            setVisitedAt(null);
                        }
                    } else {
                        console.log('suppressDate is true, skipping visitedAt fetch');
                        setVisitedAt(null);
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
                                console.log('Received ratings data:', ratingsData);
                                const ratingItems = ratingsData.items || [];
                                const userTitleRating = ratingItems.find(item => item.titleId === tconst);
                                console.log('Found user rating for this title:', userTitleRating);
                                setUserRating(userTitleRating ? userTitleRating.rating : null);
                            } else {
                                console.log('No ratings found or failed to fetch ratings:', ratingsResp.status);
                                setUserRating(null);
                            }
                        } catch (ratingErr) {
                            console.error('Error fetching rating:', ratingErr);
                            setUserRating(null);
                        }
                    } else {
                        console.log('suppressRating is true, skipping userRating fetch');
                        setUserRating(null);
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
            console.log('No tconst provided');
            setLoading(false);
        }
    }, [tconst]);

    if (loading) return <div className="title-loading">Loading...</div>;
    if (error) return <div className="title-error">Error: {error}</div>;
    if (!title) return null;

    console.log('Rendering with userRating:', userRating, 'visitedAt:', visitedAt);

    return (
        <div className='title-item-container'>
            <div className='title-item-card' key={title.id || tconst}>
                <div className='title-poster'>
                    <Link to={`/title/${title.id || tconst}`} onClick={() => HistoryHandler(title.id || tconst)}>
                        <img src={title.poster_url || title.posterUrl} alt={title.title || title.primaryTitle} />
                    </Link>
                </div>
                <Link to={`/title/${title.id || tconst}`} className='title-link' onClick={() => HistoryHandler(title.id || tconst)}>
                    <p className='title-name'>{title.title || title.primaryTitle}</p>
                    <div className='title-details'>
                        <p>Rating: {title.rating || 'N/A'}</p>
                        <p className="year">Year: {title.year || title.startYear || 'N/A'}</p>
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
