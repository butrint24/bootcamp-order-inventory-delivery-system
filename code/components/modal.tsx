import { FC, ReactNode } from "react";
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
    <Dialog open={isOpen} onClose={onClose} className="fixed z-50 inset-0 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4">
        <Dialog.Overlay className="fixed inset-0 bg-black/30" />

        <div className="relative bg-white rounded-lg max-w-md mx-auto p-6 z-10">
          <Dialog.Title className="text-lg font-bold mb-2">{title}</Dialog.Title>
          <Dialog.Description className="text-sm text-muted-foreground mb-4">
            {description}
          </Dialog.Description>

          <div className="flex justify-end gap-2">
            <Button variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button onClick={onConfirm}>Confirm</Button>
          </div>
        </div>
      </div>
    </Dialog>
  );
};
