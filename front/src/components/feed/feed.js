import { MoreVert } from "@mui/icons-material";
import { Box, IconButton, LinearProgress, Menu, MenuItem } from "@mui/material";
import React, { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppContext } from "../../context";
import { sortDates } from "../../utils/utils";
import { AddPostModal } from "./add-post-modal";
import { EditPostModal } from "./edit-post-modal";
import { FeedPost } from "./feed-post";

export const Feed = () => {
  const { user, setContextPosts } = useContext(AppContext);
  const [isLoading, setIsLoading] = useState(true);
  const [posts, setPosts] = useState([]);
  const [comments, setComments] = useState([]);
  const navigateTo = useNavigate();

  const onCreate = (createdPost) => {
    const newPosts = [...posts, createdPost];
    setPosts(sortDates(newPosts));
  };

  const fetchPosts = async () => {
    setIsLoading(true);
    try {
      const response = await fetch("https://vlogapidankaz.azurewebsites.net/api/posts", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });
      const data = await response.json();
      setPosts(sortDates(data));
      data.forEach((post) => fetchComments(post.id));
      setIsLoading(false);
    } catch (e) {
      console.log(e);
    }
  };

  const fetchComments = async (postId) => {
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${postId}/comments`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );
      const data = await response.json();
      setComments((comments) => ({
        ...comments,
        [postId]: data,
      }));
    } catch (e) {
      console.log(e);
    }
  };

  const getCommentsByPostId = (postId) => {
    return comments[postId];
  };

  useEffect(() => {
    fetchPosts();
  }, []);

  if (isLoading) return <LinearProgress sx={{ width: "100%" }} />;

  return (
    <>
      <Box flex={4} p={2} sx={{ height: "100%", cursor: "pointer" }}>
        {posts.map((post) => {
          return <FeedPost key={post.id} {...post} comments={getCommentsByPostId(post.id)} />;
        })}
      </Box>
      <AddPostModal onCreate={onCreate} />
    </>
  );
};
