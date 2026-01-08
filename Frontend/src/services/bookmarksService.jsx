// Bookmarks API service functions

const NL_API = 'https://www.newtlike.com:3000';

export const fetchBookmarks = async (username) => {
  if (!username) {
    throw new Error('Username is required to fetch bookmarks');
  } try {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      throw new Error('Not authenticated - please login again');
    }
    
    const res = await fetch(`${NL_API}/api/users/${username}/bookmarks`, { 
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });

    if (!res.ok) {
      const errorText = await res.text().catch(() => 'Unable to read error');
      throw new Error(`Failed to fetch bookmarks: ${res.status} - ${errorText}`);
    }

    const bookmarksData = await res.json();
    const items = Array.isArray(bookmarksData.items) ? bookmarksData.items : [];
    
    
    const titleDataArray = await Promise.all(items.map(async (item) => {
      try {
        const url = `${NL_API}/api/titles/${item.titleId}`;
        const resp = await fetch(url);
        if (!resp.ok) return null;
        return await resp.json();
      } catch (err) {
        return null;
      }
    }));

    // Merge bookmark items with title data
    const bookmarksWithTitles = items.map((item, idx) => {
      const titleData = titleDataArray[idx];
      return {
        ...item,
        title: titleData?.title ?? titleData?.Title ?? item.titleName ?? item.titleId,
        posterUrl: titleData?.posterUrl ?? titleData?.PosterUrl ?? '',
        rating: titleData?.rating ?? titleData?.Rating ?? null, 
        plot: titleData?.plot ?? titleData?.Plot ?? null,
        viewedAt: item.viewedAt 
      };
    });

    return bookmarksWithTitles;

  } catch (err) {
    throw err;
  }
};

export const addBookmark = async (username, titleId) => {
  if (!username) {
    throw new Error('User must be logged in to add bookmarks');
  }

  try {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      throw new Error('Not authenticated - please login again');
    }
    
    const url = `${NL_API}/api/users/${username}/bookmarks`;

    const response = await fetch(url, { 
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ username, titleId }),
      credentials: 'include'
    }); 

    if (!response.ok) {
      throw new Error('Failed to add bookmark');
    } 
    
    return true;
  } catch (error) {
    throw error;
  }
};

export const removeBookmark = async (username, titleId) => {
  if (!username) {
    throw new Error('User must be logged in to remove bookmarks');
  }

  try {
    const token = localStorage.getItem('authToken');

    if (!token) {
      throw new Error('Not authenticated - please login again');
    }
   
    const url = `${NL_API}/api/users/${username}/bookmarks/${titleId}`;

    const response = await fetch(url, { 
      method: 'DELETE', 
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ Username: username, TitleId: titleId })
    }); 

    if (!response.ok) {
      throw new Error('Failed to remove bookmark');
    }

    return true;
  } catch (error) {
    throw error;
  }
};

export const checkIfBookmarked = async (username, titleId) => {
  if (!username || !titleId) return false;

  try {
    const token = localStorage.getItem('authToken');
    
    if (!token) return false;
    
    const res = await fetch(`${NL_API}/api/users/${username}/bookmarks`, { 
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });

    if (!res.ok) {
      return false;
    }

    const bookmarksData = await res.json();
    const items = Array.isArray(bookmarksData.items) ? bookmarksData.items : [];
    
    return items.some(item => item.titleId === titleId);
  } catch (err) {
    return false;
  }
};
