import { ExpandMoreOutlined, MoreVert } from "@mui/icons-material";
import {
  Avatar,
  Box,
  Button,
  ButtonGroup,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Collapse,
  Divider,
  IconButton,
  LinearProgress,
  Menu,
  MenuItem,
  styled,
  TextField,
  Typography,
} from "@mui/material";
import React, { useContext, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { AppContext } from "../../context";
import { stringToColor } from "../../utils/utils";
import { Comment } from "../comment";
import { EditPostModal } from "../feed/edit-post-modal";
import styles from "./styles.module.css";

export const Post = () => {
  const { user } = useContext(AppContext);
  const { postId } = useParams();
  const [expanded, setExpanded] = useState(false);
  const [post, setPost] = useState(null);
  const [postComments, setPostComments] = useState([]);
  const [likes, setLikes] = useState([]);
  const [writeCommentText, setWriteCommentText] = useState("");
  const [isEditing, setIsEditing] = useState(null);
  const [isPostLoading, setIsPostLoading] = useState(true);
  const [isCommentsLoading, setIsCommentsLoading] = useState(false);
  const [isLikesLoading, setIsLikesLoading] = useState(false);
  const [anchorEl, setAnchorEl] = useState(null);
  const navigateTo = useNavigate();

  useEffect(() => {
    fetchPost();
    fetchPostComments();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => {
      setExpanded(true);
    }, 400);
    return () => clearTimeout(timer);
  }, []);

  const fetchPost = async () => {
    setIsPostLoading(true);
    try {
      const response = await fetch(`https://vlogapidankaz.azurewebsites.net/api/posts/${postId}`, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });
      const data = await response.json();
      setPost(data);
    } catch (e) {
      console.log(e);
    }
    setIsPostLoading(false);
  };

  const fetchPostComments = async () => {
    setIsCommentsLoading(true);
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
      setPostComments(data);
      data.forEach((comment) => fetchLikes(comment.id));
    } catch (e) {
      console.log(e);
    }
    setIsCommentsLoading(false);
  };

  const fetchLikes = async (commentId) => {
    setIsLikesLoading(true);
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${postId}/comments/${commentId}/likes`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${user.token}`,
          },
        }
      );
      const data = await response.json();
      setLikes((injuries) => ({
        ...injuries,
        [commentId]: data,
      }));
    } catch (e) {
      console.log(e);
    }
    setIsLikesLoading(false);
  };

  const handleExpandClick = () => {
    setExpanded(!expanded);
  };

  const ExpandMore = styled((props) => {
    const { expand, ...other } = props;
    return <IconButton {...other} />;
  })(({ theme, expand }) => ({
    transform: !expand ? "rotate(0deg)" : "rotate(180deg)",
    marginLeft: "auto",
    transition: theme.transitions.create("transform", {
      duration: theme.transitions.duration.shortest,
    }),
  }));

  if (isPostLoading || isCommentsLoading || isLikesLoading) {
    return <LinearProgress sx={{ width: "100%" }} />;
  }

  const closeUpdateModal = () => {
    setIsEditing(false);
  };

  const openMoreOptions = Boolean(anchorEl);
  const handleClick = (e) => {
    setAnchorEl(e.currentTarget);
  };

  const onUpdate = (updatedPost) => {
    setPost(updatedPost);
  };

  const handleDelete = async () => {
    try {
      const response = await fetch(`https://vlogapidankaz.azurewebsites.net/api/posts/${postId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });
      navigateTo("/");
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

  const handlePostComment = async () => {
    const createdComment = { Content: writeCommentText };
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${postId}/comments`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify(createdComment),
        }
      );
      const data = await response.json();
      // setPostComments(postComments.concat(data));
      setPostComments([...postComments, data]);
      setWriteCommentText("");
    } catch (e) {
      console.log(e);
    }
  };

  const onCommentUpdate = (updatedComment) => {
    const updatedComments = postComments.map((postComment) =>
      postComment.id === updatedComment.id ? updatedComment : postComment
    );
    setPostComments(updatedComments);
  };

  const onCommentDelete = (deletedCommentId) => {
    const newComments = postComments.filter((comment) => comment.id !== deletedCommentId);
    setPostComments(newComments);
  };

  return (
    <>
      <Box flex={4} p={2} sx={{ height: "100%" }}>
        <Card sx={{ margin: 3 }}>
          <CardHeader
            sx={{ paddingRight: "25px" }}
            avatar={
              <Avatar sx={{ bgcolor: `${stringToColor(post.username)}` }}>
                {post.username.charAt(0)}
              </Avatar>
            }
            action={
              (user.name.toUpperCase() === post.username ||
                user.roles.find((role) => role === "Admin")) && (
                <IconButton edge="end" onClick={handleClick}>
                  <MoreVert />
                </IconButton>
              )
            }
            title={
              <Typography variant="h7" sx={{ wordWrap: "break-word" }}>
                {post.username}
              </Typography>
            }
            subheader={new Date(post.creationDate).toLocaleDateString("zh-Hans-CN", {
              hour: "2-digit",
              minute: "2-digit",
            })}
          />
          <CardContent sx={{ paddingTop: 0 }}>
            <Divider />
            <Typography
              variant="h5"
              color="text.primary"
              sx={{ marginTop: "10px", wordWrap: "break-word" }}
            >
              {post.name}
            </Typography>
            <Typography
              variant="body1"
              color="text.primary"
              sx={{ marginTop: "20px", display: "flex" }}
            >
              {post.body}
            </Typography>
            <Typography
              variant="body2"
              color="text.secondary"
              sx={{ marginTop: "25px", wordWrap: "break-word" }}
            >
              {!!postComments.length ? postComments.length : 0} comments
            </Typography>
          </CardContent>
          <CardActions disableSpacing sx={{ paddingTop: 0, height: "26px" }}>
            <ExpandMore expand={expanded} onClick={handleExpandClick}>
              <ExpandMoreOutlined />
            </ExpandMore>
          </CardActions>
          <Collapse in={expanded} timeout="auto" unmountOnExit>
            <CardContent sx={{ paddingTop: 0 }}>
              <Divider sx={{ bgcolor: "#1565c0" }} />
              <Typography
                variant="h5"
                color="text.primary"
                sx={{ marginTop: "15px", paddingLeft: "20px", wordWrap: "break-word" }}
              >
                {!!postComments.length ? "Comments" : "No comments yet"}
              </Typography>
              <Box
                width={"100%"}
                height={150}
                bgcolor={"background.default"}
                color={"text.primary"}
                p={3}
                borderRadius={5}
              >
                <UserBox sx={{ marginTop: "20px" }}>
                  <div className={styles.commentPost}>
                    <Avatar
                      sx={{
                        bgcolor: `${stringToColor(user.name.toUpperCase())}`,
                        marginTop: "16px",
                      }}
                    >
                      {user.name.charAt(0).toUpperCase()}
                    </Avatar>
                    <TextField
                      sx={{ width: "100%" }}
                      label="Write a comment..."
                      multiline
                      maxRows={5}
                      value={writeCommentText}
                      variant="standard"
                      placeholder="Comment"
                      onChange={(e) => {
                        setWriteCommentText(e.target.value);
                      }}
                    />
                  </div>
                  <ButtonGroup variant="contained" fullWidth sx={{ width: "30%" }}>
                    <Button onClick={handlePostComment} disabled={writeCommentText === ""}>
                      Comment
                    </Button>
                  </ButtonGroup>
                </UserBox>
              </Box>
              {!!postComments.length ? (
                <Typography variant="body1" color="text.primary" sx={{ wordWrap: "break-word" }}>
                  {postComments.map((comment) => {
                    return (
                      <Comment
                        key={comment.id}
                        comment={comment}
                        onCommentUpdate={onCommentUpdate}
                        onCommentDelete={onCommentDelete}
                        likes={likes[comment.id]}
                      />
                    );
                  })}
                </Typography>
              ) : null}
            </CardContent>
          </Collapse>
        </Card>
      </Box>
      {!!isEditing && <EditPostModal post={post} onUpdate={onUpdate} close={closeUpdateModal} />}
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
  );
};

const UserBox = styled(Box)({
  display: "flex",
  flexDirection: "column",
  alignItems: "flex-end",
  marginBottom: "40px",
  gap: "5px",
});
