import React, { useContext } from "react";
import {
  Avatar,
  Button,
  ButtonGroup,
  Fab,
  Modal,
  styled,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import { Add } from "@mui/icons-material";
import { Box } from "@mui/system";
import { useState } from "react";
import { AppContext } from "../../../context";
import { stringToColor } from "../../../utils/utils";

export const EditCommentModal = ({ comment, onUpdate, close }) => {
  const { user } = useContext(AppContext);
  const [isLoading, setIsLoading] = useState(false);
  const [postComment, setPostComment] = useState(comment.content);

  const updateComment = async () => {
    setIsLoading(true);
    const updatedComment = { Content: postComment };
    try {
      const response = await fetch(
        `https://vlogapidankaz.azurewebsites.net/api/posts/${comment.postId}/comments/${comment.id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${user.token}`,
          },
          body: JSON.stringify(updatedComment),
        }
      );
      const data = await response.json();
      onUpdate(data);
      close();
    } catch (e) {
      console.log(e);
    }
    setIsLoading(false);
  };

  return (
    <>
      <StyledModal open={true} onClose={close}>
        <Box
          width={430}
          height={290}
          bgcolor={"background.default"}
          color={"text.primary"}
          p={3}
          borderRadius={5}
        >
          <Typography variant="h6" textAlign="center">
            Edit Comment
          </Typography>
          <UserBox sx={{ marginTop: "15px" }}>
            <Avatar sx={{ bgcolor: `${stringToColor(user.name.toUpperCase())}` }}>
              {user.name.charAt(0).toUpperCase()}
            </Avatar>
            <Typography fontWeight={500} variant="span" sx={{ wordWrap: "break-word" }}>
              {user.name.toUpperCase()}
            </Typography>
          </UserBox>
          <TextField
            sx={{ width: "100%", marginTop: "20px" }}
            label="Title"
            multiline
            maxRows={1}
            value={postComment}
            placeholder="Comment"
            onChange={(e) => {
              setPostComment(e.target.value);
            }}
          />
          <ButtonGroup variant="contained" fullWidth sx={{ marginTop: "20px" }}>
            <Button disabled={postComment === ""} onClick={updateComment}>
              Update
            </Button>
          </ButtonGroup>
        </Box>
      </StyledModal>
    </>
  );
};

const StyledModal = styled(Modal)({
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
});

const UserBox = styled(Box)({
  display: "flex",
  gap: "10px",
  alignItems: "center",
  marginBottom: "20px",
});
