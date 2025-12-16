// Rating API service functions
export const fetchUserRating = async (user, titleId) => {
  if (!user || !titleId) return 0;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) return 0;
    const username = user.username || user.Username;
    const res = await fetch(`https://newtlike.com:3000/api/users/${username}/ratings/${titleId}`, {
      headers: { 
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    if (res.ok) {
      const data = await res.json();
      return data.rating || 0;
    }
    if (res.status === 404) return 0;
    if (res.status === 401) {
      alert('Session expired. Please login again.');
    }
    return 0;
  } catch (err) {
    return 0;
  }
};

export const submitRating = async (user, titleId, rating) => {
  if (!user) {
    alert('Please login to rate');
    return false;
  }
  try {
    const token = localStorage.getItem('authToken');
    if (!token) {
      alert('No authentication token found. Please login again.');
      return false;
    }
    
    const username = user.username || user.Username;
    
    // Check if rating exists without triggering auth error
    const checkRes = await fetch(`https://newtlike.com:3000/api/users/${username}/ratings/${titleId}`, {
      headers: { 
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const hasExistingRating = checkRes.ok;
    const method = hasExistingRating ? 'PUT' : 'POST';
    const url = `https://newtlike.com:3000/api/users/${username}/ratings${hasExistingRating ? `/${titleId}` : ''}`;
    
    const res = await fetch(url, {
      method,
      headers: { 
        'Authorization': `Bearer ${token}`, 
        'Content-Type': 'application/json' 
      },
      body: JSON.stringify({ Username: username, TitleId: titleId, Rating: rating })
    });
    
    if (!res.ok) {
      if (res.status === 401) {
        alert('Session expired. Please login again.');
      } else {
        const errorData = await res.json().catch(() => ({}));
        alert(`Failed to submit rating: ${errorData.error || res.statusText}`);
      }
      return false;
    }
    
    return true;
  } catch (err) {
    alert('An error occurred while submitting your rating.');
    return false;
  }
};

export const deleteRating = async (user, titleId) => {
  if (!user) return false;
  if (!confirm('Delete your rating?')) return false;
  
  try {
    const token = localStorage.getItem('authToken');
    const username = user.username || user.Username;
    const res = await fetch(`https://newtlike.com:3000/api/users/${username}/ratings/${titleId}`, {
      method: 'DELETE',
      headers: { 
        'Authorization': `Bearer ${token}`, 
        'Content-Type': 'application/json' 
      },
      body: JSON.stringify({ username, titleId })
    });
    
    return res.ok;
  } catch (err) {
    return false;
  }
};

export const fetchAllUserRatings = async (username) => {
  if (!username) {
    throw new Error('Username is required to fetch ratings');
  }

  try {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      throw new Error('Not authenticated - please login again');
    }
    
    const res = await fetch(`https://newtlike.com:3000/api/users/${username}/ratings`, { 
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });

    if (!res.ok) {
      throw new Error(`Failed to fetch ratings: ${res.status}`);
    }

    const ratingsData = await res.json();
    const items = Array.isArray(ratingsData.items) ? ratingsData.items : [];
    
    
    const titleDataArray = await Promise.all(items.map(async (item) => {
      try {
        const url = `https://newtlike.com:3000/api/titles/${item.titleId}`;
        const resp = await fetch(url);
        if (!resp.ok) return null;
        return await resp.json();
      } catch (err) {
        return null;
      }
    }));

    // Merge rating items with title data
    const ratingsWithTitles = items.map((item, idx) => {
      const titleData = titleDataArray[idx];
      return {
        ...item,
        title: titleData?.title ?? titleData?.Title ?? item.titleName ?? item.titleId,
        posterUrl: titleData?.posterUrl ?? titleData?.PosterUrl ?? '',
        rating: item.rating || item.Rating || 0
      };
    });

    return ratingsWithTitles;

  } catch (err) {
    throw err;
  }
};