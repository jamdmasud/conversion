public ResponseMessage Pagination<T>(ResponseMessage responseMessage, Message message, List<T> listObj)
        {

            if (Convert.ToInt32(message.PageIndex) > 0)
            {
                responseMessage.RecordCount = listObj.Count / Convert.ToInt32(message.PageSize);

                if (listObj.Count % Convert.ToInt32(message.PageSize) != 0)
                    responseMessage.RecordCount++;

                //pagination
                if (Convert.ToInt32(message.PageIndex) > responseMessage.RecordCount)
                    message.PageIndex = "1";
                int pageSize = Convert.ToInt32(message.PageSize);
                int index = 0;
                if (Convert.ToInt32(message.PageIndex) > 0)
                    index = (Convert.ToInt32(message.PageIndex) - 1) * Convert.ToInt32(message.PageSize);


                if (listObj.Count <= index + Convert.ToInt32(message.PageSize))
                {
                    pageSize = listObj.Count - index;
                }
                listObj = listObj.GetRange(index, pageSize);
            }
            responseMessage.ResponseObj = listObj;
            responseMessage.PageIndex = Convert.ToInt32(message.PageIndex);
            return responseMessage;
        }
