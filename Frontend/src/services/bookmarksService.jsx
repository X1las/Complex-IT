// Bookmarks API service functions
const NL_API = 'https://www.newtlike.com:3000';


export const fetchBookmarks = async (username) => {
  

  if (!username) {
    throw new Error('Username is required to fetch bookmarks');
  }

  console.log('fetchBookmarks called for username:', username);

  try {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      console.error('No auth token found');
      throw new Error('Not authenticated - please login again');
    }
    
    console.log('Making request to:', `${NL_API}/api/users/${username}/bookmarks`);
    
    const res = await fetch(`${NL_API}/api/users/${username}/bookmarks`, { 
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      credentials: 'include'
    });

    console.log('Response status:', res.status);

    if (!res.ok) {
      const errorText = await res.text().catch(() => 'Unable to read error');
      console.error('Failed to fetch bookmarks:', res.status, errorText);
      throw new Error(`Failed to fetch bookmarks: ${res.status} - ${errorText}`);
    } 
       
    
    const bookmarksData = await res.json();
    console.log('Raw bookmarks data:', bookmarksData);
    
    const items = Array.isArray(bookmarksData.items) ? bookmarksData.items : [];
    console.log('Processed bookmark items:', items.map(i => ({ titleId: i.titleId, viewedAt: i.viewedAt })));
    
    // Fetch title data for each bookmark
    const titleDataArray = await Promise.all(items.map(async (item) => {
      try {
        const url = `${NL_API}/api/titles/${item.titleId}`;
        const resp = await fetch(url);
        if (!resp.ok) {
          console.error(`Failed to fetch title data for ${item.titleId}:`, resp.status);
          return null;
        }
        return await resp.json();
      } catch (err) {
        console.error(`Error fetching title ${item.titleId}:`, err);
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

    console.log('Fetched title data for bookmarks:', bookmarksWithTitles);
    return bookmarksWithTitles;

  } catch (err) {
    console.error('Error fetching bookmarks:', err);
    throw err;
  }
};

export const addBookmark = async (username, titleId) => {
  if (!username) {
    console.error('No user logged in');
    throw new Error('User must be logged in to add bookmarks');
  }

  try {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      console.error('No auth token found');
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
      const txt = await response.text().catch(() => '');
      console.error('Failed to add bookmark:', response.status, txt.slice(0, 300));
      throw new Error('Failed to add bookmark');
    } 
    
    console.log(`Added ${titleId} to bookmarks for ${username}`);
    return true;
  } catch (error) {
    console.error('Error adding bookmark:', error);
    throw error;
  }
};

export const removeBookmark = async (username, titleId) => {
  if (!username) {
    console.error('No user logged in');
    throw new Error('User must be logged in to remove bookmarks');
  }

  try {
    const token = localStorage.getItem('authToken');

    if (!token) {
      console.error('No auth token found');
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
      const txt = await response.text().catch(() => '');
      console.error('Failed to remove bookmark:', response.status, txt);
      throw new Error('Failed to remove bookmark');
    }

    return true;
  } catch (error) {
    console.error('Error removing bookmark:', error);
    throw error;
  }
};

export const checkIfBookmarked = async (username, titleId) => {
  if (!username || !titleId) return false;

  try {
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      console.warn('No auth token found');
      return false;
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
      return false;
    }

    const bookmarksData = await res.json();
    const items = Array.isArray(bookmarksData.items) ? bookmarksData.items : [];
    
    return items.some(item => item.titleId === titleId);
  } catch (err) {
    console.error('Error checking bookmark status:', err);
    return false;
  }
};