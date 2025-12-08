import React, { useState, useEffect } from 'react';

export  const useAddTitleToHistory = () => {
  const {user} = useAuth();

  const addToHistory = async (titleId) => {
    if (!user) {
      console.error('No user logged in');
      return;
    }

    try {
      const token = localStorage.getItem('authToken');
      
      if (!token) {
        console.error('No auth token found');
        return;
      }
      const url = `${NL_API}/api/users/${user.username}/history/${titleId}`;

      const response = await fetch(url, { 
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({username:user.username, titleId:titleId}), credentials: 'include'
      });

      console.log('Add to history response status:', response.status);

      if (!response.ok) {
        const txt = await response.text().catch(() => '');
        console.error('Failed to add to history:', response.status, txt.slice(0, 300));
        return;
      }
         console.log(`Added ${titleId} to history for ${user.username}`);
      return true;
    } catch (error) {
      console.error('Error adding to history:', error);
      return false;
    }
  };

  return { addToHistory };
};

const Bookmarks = () => {

  return (
    <div className="bookmarks-container">
      <h1>Bookmarks</h1>
      <p>kjahsgdkjhasd</p>
      {/* TODO: Add bookmarks UI */}
    </div>
  );
};

export default Bookmarks;
