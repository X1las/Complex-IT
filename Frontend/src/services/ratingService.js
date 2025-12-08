// Rating API service functions

export const fetchUserRating = async (user, titleId) => {
  if (!user || !titleId) return 0;
  try {
    const token = localStorage.getItem('authToken');
    if (!token) {
      console.warn('No auth token found');
      return 0;
    }
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
    if (res.status === 404) {
      return 0; // No rating exists yet
    }
    if (res.status === 401) {
      console.error('Unauthorized - token may be invalid');
      alert('Session expired. Please login again.');
    }
    return 0;
  } catch (err) {
    console.error('Error fetching rating:', err);
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
    console.error('Error submitting rating:', err);
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
    console.error('Error deleting rating:', err);
    return false;
  }
};