import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";
import { useParams } from "react-router-dom";
import {
  getPlanProcedureUsers,
  addUserToProcedure,
  removeUserFromProcedure,
}from "../../../api/api"

const PlanProcedureItem = ({ procedure, users }) => {
    let { id } = useParams();
    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(() => {
      (async () => {
          var getselectedUsers = await getPlanProcedureUsers(id, procedure.procedureId);
          const initialSelectedUsers = getselectedUsers.map((user) => ({
              label: user.user.name,
              value: user.user.userId,
          }));
          setSelectedUsers(initialSelectedUsers);
      })();
    }, [procedure, id]);


    const handleAssignUserToProcedure = async (selectedOptions) => {
      const newUser = selectedOptions.find(option => !selectedUsers.includes(option));
      if (newUser) {
          await addUserToProcedure(id, procedure.procedureId, newUser.value);
      }
      setSelectedUsers(selectedOptions);
    };

    const handleRemoveUserFromProcedure = async (selectedOptions) => {
        const removedUser = selectedUsers.find(option => !selectedOptions.includes(option));
        if (removedUser) {
            const removedUsers = selectedUsers.filter(user => !selectedOptions.includes(user));
            var clearAll = removedUsers.length == selectedUsers.length;
            await removeUserFromProcedure(id, procedure.procedureId, removedUser.value, clearAll);
        }
        
        setSelectedUsers(selectedOptions);
    };

    const handleChange = async (selectedOptions) => {
        if (selectedOptions.length < selectedUsers.length) {
            await handleRemoveUserFromProcedure(selectedOptions);
        } else {
            await handleAssignUserToProcedure(selectedOptions);
        }
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleChange(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
