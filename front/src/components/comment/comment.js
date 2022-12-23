import { MoreVert } from "@mui/icons-material";
import {
  Avatar,
  Card,
  CardContent,
  CardHeader,
  IconButton,
  LinearProgress,
  Menu,
  MenuItem,
  Typography,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { AppContext } from "../../context";
import { stringToColor } from "../../utils/utils";
import { Like } from "../like";
import { EditCommentModal } from "./edit-comment-modal";

export const Comment = ({ comment, onCommentUpdate, onCommentDelete }) => {
  const { user } = useContext(AppContext);
  const { postId } = useParams();
  const [anchorEl, setAnchorEl] = useState(null);
  const [isEditing, setIsEditing] = useState(null);
  const [isLikesLoading, setIsLikesLoading] = useState(false);
  const [likes, setLikes] = useState([]);
  const [like, setLike] = useState(null);

  useEffect(() => {
    fetchCommentLikes();
  }, []);

  const fetchCommentLikes = async () => {
    setIsLikesLoading(true);
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${postId}/comments/${comment.id}/likes`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );
      const data = await response.json();
      setLikes(data);
      setLike(
        data.length > 0
          ? data.find((like) => like.normalizedUsername === user.name.toUpperCase())
          : null
      );
    } catch (e) {
      console.log(e);
    }
    setIsLikesLoading(false);
  };

  const sendLikeRequest = async (type) => {
    const createdLike = { IsPositive: type };
    setIsLikesLoading(true);
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${comment.postId}/comments/${comment.id}/likes`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify(createdLike),
        }
      );
      const data = await response.json();
      setLikes((prevLikes) => likes.concat(data));
      setLike(data);
    } catch (e) {
      console.log(e);
    }
    setIsLikesLoading(false);
  };

  const sendLikePutRequest = async () => {
    const createdLike = { IsPositive: !like.IsPositive };
    setIsLikesLoading(true);
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${comment.postId}/comments/${comment.id}/likes/${like.id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify(createdLike),
        }
      );
      const data = await response.json();
      const index = likes.findIndex((l) => l.id === like.id);
      if (index >= 0) {
        const newLikes = [...likes];
        newLikes[index].isPositive = !newLikes[index].isPositive;
        setLikes(newLikes);
        setLike(
          newLikes.length > 0
            ? data.find((like) => like.normalizedUsername === user.name.toUpperCase())
            : null
        );
      }
    } catch (e) {
      console.log(e);
    }
    setIsLikesLoading(false);
  };

  const deleteLikeRequest = async () => {
    if (like === null) return;
    setIsLikesLoading(true);
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${comment.postId}/comments/${comment.id}/likes/${like.id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );
      setLikes((prevLikes) => prevLikes.filter((data) => data.id !== like.id));
      setLike(null);
    } catch (e) {
      console.log(e);
    }
    setIsLikesLoading(false);
  };

  const closeUpdateModal = () => {
    setIsEditing(false);
  };

  const openMoreOptions = Boolean(anchorEl);
  const handleClick = (e) => {
    setAnchorEl(e.currentTarget);
  };

  const onUpdate = (updatedComment) => {
    onCommentUpdate(updatedComment);
  };

  const handleDelete = async () => {
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${comment.postId}/comments/${comment.id}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );
      onCommentDelete(comment.id);
    } catch (e) {
      console.log(e);
    }
    setAnchorEl(null);
  };

  const handleEdit = () => {
    setIsEditing(true);
    setAnchorEl(null);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const onUpvote = () => {
    if (like === null || like === undefined) {
      sendLikeRequest(true);
      return;
    }
    if (like.isPositive === false) {
      sendLikePutRequest();
      return;
    }
    if (like.isPositive === true) {
      deleteLikeRequest();
      return;
    }
  };

  const onDownvote = () => {
    if (like === null || like === undefined) {
      sendLikeRequest(false);
      return;
    }
    if (like.isPositive === true) {
      sendLikePutRequest();
      return;
    }
    if (like.isPositive === false) {
      deleteLikeRequest();
      return;
    }
  };

  let totalRating = 0;

  if (isLikesLoading) return <LinearProgress sx={{ width: "100%" }} />;

  if (likes.length > 0) {
    totalRating = likes.reduce((sum, like) => {
      return sum + (like.isPositive ? 1 : -1);
    }, 0);
  }

  return (
    <>
      {comment.content === null ? (
        <></>
      ) : (
        <>
          <Card sx={{ margin: 3, bgcolor: "dark light" }}>
            <CardHeader
              avatar={
                <Avatar sx={{ bgcolor: `${stringToColor(comment.username)}` }}>
                  {comment.username.charAt(0)}
                </Avatar>
              }
              action={
                (user.name.toUpperCase() === comment.username ||
                  (user.roles.length !== 0 && user.roles.find((role) => role === "Admin"))) && (
                  <IconButton edge="end" onClick={handleClick}>
                    <MoreVert />
                  </IconButton>
                )
              }
              title={
                <Typography variant="h7" sx={{ wordWrap: "break-word" }}>
                  {comment.username}
                </Typography>
              }
              subheader={new Date(comment.creationDate).toLocaleDateString("zh-Hans-CN", {
                hour: "2-digit",
                minute: "2-digit",
              })}
            />
            <CardContent>
              <Typography variant="body1" color="text.primary" sx={{ wordWrap: "break-word" }}>
                {comment.content}
              </Typography>
              <Like
                votes={totalRating}
                handleUpvote={onUpvote}
                handleDownvote={onDownvote}
                value={!!like ? (like.isPositive ? 1 : -1) : 0}
              />
            </CardContent>
          </Card>
          {!!isEditing && (
            <EditCommentModal comment={comment} onUpdate={onUpdate} close={closeUpdateModal} />
          )}
          <Menu
            open={openMoreOptions}
            anchorEl={anchorEl}
            onClose={handleClose}
            anchorOrigin={{
              vertical: "top",
              horizontal: "right",
            }}
            transformOrigin={{
              vertical: "top",
              horizontal: "right",
            }}
          >
            <MenuItem onClick={handleEdit}>Edit</MenuItem>
            <MenuItem onClick={handleDelete}>Delete</MenuItem>
          </Menu>
        </>
      )}
    </>
  );
};
