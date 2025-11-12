import { FC } from "react";
import { Dialog } from "@headlessui/react";
import { Button } from "@/components/ui/button";

interface ConfirmModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title?: string;
  description?: string;
}

export const ConfirmModal: FC<ConfirmModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
  title = "Confirm Purchase",
  description = "Are you sure you want to purchase these items?",
}) => {
  return (
    <Dialog open={isOpen} onClose={onClose} className="relative z-50">
      <div className="fixed inset-0 bg-black/30" aria-hidden="true" />

      <div className="fixed inset-0 flex items-center justify-center p-4">
        <Dialog.Panel className="bg-white rounded-lg max-w-md mx-auto p-6 shadow-lg">
          <Dialog.Title className="text-lg font-bold mb-2">{title}</Dialog.Title>
          <Dialog.Description className="text-sm text-gray-600 mb-4">
            {description}
          </Dialog.Description>

          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button onClick={onConfirm}>Confirm</Button>
          </div>
        </Dialog.Panel>
      </div>
    </Dialog>
  );
};
